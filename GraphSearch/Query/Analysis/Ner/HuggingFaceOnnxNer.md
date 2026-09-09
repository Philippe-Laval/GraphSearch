# Claude explanation

This one's substantially more involved — HuggingFace token classification models require BERT/SentencePiece tokenization plus BIO post-processing. I'll:
1.	Add Microsoft.ML.OnnxRuntime.
2.	Define a tokenizer abstraction IHuggingFaceTokenizer so you can plug in Microsoft.ML.Tokenizers (whose API keeps evolving) or any other tokenizer without churn.
3.	Implement HuggingFaceOnnxNerService — handles ONNX inference, argmax + softmax confidence, and BIO span aggregation with char offsets.
4.	Implement HuggingFaceOnnxNerServicePool — routes EN → English model, FR → French model.
5.	Reuse WhWordLexicon + RelationalVerbLexicon (same trick as Azure) since HF NER models don't emit lemmas or POS.

New pieces
-	NuGet: Microsoft.ML.OnnxRuntime 1.19.2 added.
-	IHuggingFaceTokenizer + HuggingFaceEncoding + CharSpan — small abstraction over the tokenizer so the code is stable regardless of which C# tokenizer package (or version) you plug in.
-	HuggingFaceOnnxNerService : INerService, IDisposable — one instance per language/model:
-	Auto-detects model input names (input_ids, attention_mask, and optional token_type_ids for BERT-style vs CamemBERT-style).
-	Runs ONNX inference, does per-token argmax + softmax for confidence.
-	BIO span aggregation with char-offset back-mapping to the original text. Handles orphan I-XXX tags and no-prefix labels (PER treated as begin).
-	Skips padding (attention_mask == 0) and special tokens (marked via CharSpan.None by the tokenizer adapter).
-	Reuses WhWordLexicon + RelationalVerbLexicon for wh-word and root-verb detection (same normalization to canonical English as Azure).
-	HuggingFaceOnnxNerServicePool : INerService, IDisposable — language-routing dispatcher (EN model / FR model) driven by ILanguageDetector. Regional variants (en-US, fr-CA) collapse to base codes.
-	HuggingFaceOnnxNer.md — setup guide: model choice, optimum-cli export commands, a working Microsoft.ML.Tokenizers adapter sketch, and wire-up snippets (standalone + inside your existing hybrid composite).

Recommended models
Language	Model	Labels
English	dslim/bert-base-NER	O, B-PER, I-PER, B-ORG, I-ORG, B-LOC, I-LOC, B-MISC, I-MISC
French	Jean-Baptiste/camembert-ner	same schema

Wire-up sketch (from the md)

```csharp
var enSession = new InferenceSession("onnx/en-bert-ner/model.onnx");
var frSession = new InferenceSession("onnx/fr-camembert-ner/model.onnx");
var labels = new[] { "O","B-PER","I-PER","B-ORG","I-ORG","B-LOC","I-LOC","B-MISC","I-MISC" };

INerService ner = new HuggingFaceOnnxNerServicePool(
    byLanguage: new Dictionary<string, INerService>
    {
        ["en"] = new HuggingFaceOnnxNerService(enSession, enTokenizer, labels, "en", disposeSession: true),
        ["fr"] = new HuggingFaceOnnxNerService(frSession, frTokenizer, labels, "fr", disposeSession: true),
    },
    languageDetector: new HeuristicLanguageDetector(),
    disposeServices: true);

IQueryAnalyzer analyzer = new NerQueryAnalyzer(ner);
```

Now you have three interchangeable NER backends (spaCy microservice, Azure AI Language, HF-ONNX) 
all implementing INerService — freely composable through HybridNerAnalyzers, 
CompositeQueryAnalyzer (with continueOnException: true), and MultilingualQueryAnalyzer.




# HuggingFace ONNX NER — setup guide

The C# side lives in `GraphSearch\Query\Analysis\`:

- **`IHuggingFaceTokenizer`** — you implement (BERT vocab, CamemBERT SPM, …).
- **`HuggingFaceOnnxNerService`** — one instance per language/model. Handles ONNX inference + BIO span aggregation + softmax confidence.
- **`HuggingFaceOnnxNerServicePool`** — routes on language (`ILanguageDetector`).

Wh-word and root-verb detection reuse `WhWordLexicon` and `RelationalVerbLexicon` (same behaviour as the Azure backend — French wh-words are normalized to canonical English lemmas).

---

## 1. Export the models to ONNX

Recommended models (BIO-tagged, 4-class NER):

| Language | HuggingFace model                     | Tokenizer type |
|----------|---------------------------------------|----------------|
| English  | `dslim/bert-base-NER`                 | WordPiece (BERT vocab) |
| French   | `Jean-Baptiste/camembert-ner`         | SentencePiece (CamemBERT) |

Both are Apache-2.0 or MIT and expose the standard `[O, B-PER, I-PER, B-ORG, I-ORG, B-LOC, I-LOC, B-MISC, I-MISC]` labels.

Export once with [🤗 Optimum](https://github.com/huggingface/optimum):

```powershell
pip install "optimum[onnxruntime]"

# English
optimum-cli export onnx --model dslim/bert-base-NER onnx/en-bert-ner

# French
optimum-cli export onnx --model Jean-Baptiste/camembert-ner onnx/fr-camembert-ner
```

Each folder will contain `model.onnx`, the tokenizer files (`vocab.txt`, `tokenizer.json`, `sentencepiece.bpe.model`, …), and `config.json` (has the `id2label` mapping — copy it into your `labels` array).

---

## 2. Implement `IHuggingFaceTokenizer`

The C# side is intentionally tokenizer-agnostic. Two easy paths:

### Option A — `Microsoft.ML.Tokenizers` (in-process)

```csharp
using Microsoft.ML.Tokenizers;

// English (BERT WordPiece)
var bert = BertTokenizer.Create("onnx/en-bert-ner/vocab.txt");

// French (CamemBERT SentencePiece)
using var spmStream = File.OpenRead("onnx/fr-camembert-ner/sentencepiece.bpe.model");
var camembert = SentencePieceTokenizer.Create(spmStream);
```

Wrap with an adapter that fills `HuggingFaceEncoding`:

```csharp
public sealed class MlTokenizerAdapter(Tokenizer inner, int maxLen = 128) : IHuggingFaceTokenizer
{
	public HuggingFaceEncoding Encode(string text)
	{
		var result = inner.EncodeToTokens(text, out _);
		var ids   = new List<long>(result.Count + 2);
		var mask  = new List<long>(result.Count + 2);
		var spans = new List<CharSpan>(result.Count + 2);

		// Add [CLS] / <s>
		ids.Add(inner.ClsTokenId ?? 0);   // adjust for your model
		mask.Add(1);
		spans.Add(CharSpan.None);

		foreach (var tok in result)
		{
			ids.Add(tok.Id);
			mask.Add(1);
			spans.Add(new CharSpan(tok.Offset.Start.Value, tok.Offset.End.Value - tok.Offset.Start.Value));
			if (ids.Count >= maxLen - 1) break;
		}

		// Add [SEP] / </s>
		ids.Add(inner.SepTokenId ?? 0);
		mask.Add(1);
		spans.Add(CharSpan.None);

		return new HuggingFaceEncoding(
			ids.ToArray(),
			mask.ToArray(),
			TokenTypeIds: null,     // BERT: allocate zeros; CamemBERT: keep null
			TokenCharSpans: spans);
	}
}
```

> The exact API names in `Microsoft.ML.Tokenizers` change between versions.
> This is the reason for the abstraction — the ONNX + BIO code in `HuggingFaceOnnxNerService`
> is stable regardless.

### Option B — sidecar HTTP tokenizer

If you already run the spaCy microservice (or a HF `text-generation-inference` server), expose a `/tokenize` endpoint that returns `input_ids`, `attention_mask`, `offsets`. Then the C# adapter is a small `HttpClient` call. This avoids C# tokenizer maintenance entirely.

---

## 3. Wire it up

```csharp
using Microsoft.ML.OnnxRuntime;

var enSession = new InferenceSession("onnx/en-bert-ner/model.onnx");
var frSession = new InferenceSession("onnx/fr-camembert-ner/model.onnx");

var enLabels = new[] { "O", "B-PER", "I-PER", "B-ORG", "I-ORG", "B-LOC", "I-LOC", "B-MISC", "I-MISC" };
var frLabels = enLabels; // camembert-ner uses the same schema

var en = new HuggingFaceOnnxNerService(enSession, enTokenizer, enLabels, language: "en", disposeSession: true);
var fr = new HuggingFaceOnnxNerService(frSession, frTokenizer, frLabels, language: "fr", disposeSession: true);

INerService ner = new HuggingFaceOnnxNerServicePool(
	byLanguage: new Dictionary<string, INerService> { ["en"] = en, ["fr"] = fr },
	languageDetector: new HeuristicLanguageDetector(),
	defaultLanguage: "en",
	disposeServices: true);

IQueryAnalyzer analyzer = new NerQueryAnalyzer(ner);
```

Or drop it into the resilient hybrid you already have:

```csharp
IQueryAnalyzer analyzer = new CachingQueryAnalyzer(
	new CompositeQueryAnalyzer(
		analyzers: new IQueryAnalyzer[]
		{
			new NerQueryAnalyzer(ner),                                 // fast, local ONNX
			new NerQueryAnalyzer(azure),                               // cloud fallback
			new LlmQueryAnalyzer(chat),                                // last resort
		},
		continueOnException: true));
```

---

## 4. Notes on quality

- BERT/CamemBERT NER models are trained on **cased** text — do **not** feed lowercased input from `QueryAnalyzer`. Either pass a separate `IQueryAnalyzer` with `ToLowerCase = false` as the inner normalizer of `NerQueryAnalyzer`, or wire the ONNX service directly.
- HF token-classification models don't emit lemmas or POS. `Lemma` returns `null`; `NerQueryAnalyzer` falls back to the normalized query.
- If your model uses a non-BIO label schema (BIOES, plain tags), adjust `ParseBioLabel` accordingly.
- For latency-sensitive workloads, cap query length (`MaxSequenceLength` in your tokenizer adapter) and pre-warm the session with a dummy inference at startup.
