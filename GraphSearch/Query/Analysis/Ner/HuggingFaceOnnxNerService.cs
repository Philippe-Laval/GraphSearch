using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace GraphSearch.Library.Query.Analysis.Ner;

/// <summary>
/// <see cref="INerService"/> backed by a HuggingFace token-classification model
/// exported to ONNX (e.g. via <c>optimum-cli export onnx --model ...</c>).
///
/// Recommended models:
/// <list type="bullet">
///   <item>English: <c>dslim/bert-base-NER</c> (labels: PER, ORG, LOC, MISC).</item>
///   <item>French: <c>Jean-Baptiste/camembert-ner</c> (labels: PER, ORG, LOC, MISC).</item>
/// </list>
///
/// <para>Pipeline:</para>
/// <list type="number">
///   <item>Tokenize (via <see cref="IHuggingFaceTokenizer"/>).</item>
///   <item>Run ONNX session with <c>input_ids</c>, <c>attention_mask</c>, and
///   optionally <c>token_type_ids</c> (auto-detected from the model's input metadata).</item>
///   <item>Per-token argmax on the logits tensor; softmax for confidence.</item>
///   <item>Aggregate BIO labels into entity spans using the tokenizer's char offsets.</item>
/// </list>
///
/// <para>HF NER models do not emit lemmas or POS tags. As with the Azure
/// backend, we approximate:</para>
/// <list type="bullet">
///   <item><b>InterrogativeLemma</b> via <see cref="WhWordLexicon"/> (FR normalized to EN).</item>
///   <item><b>RootVerbs</b> via <see cref="RelationalVerbLexicon"/>.</item>
///   <item><b>Lemma</b>: <c>null</c> — <see cref="NerQueryAnalyzer"/> falls back to the normalized text.</item>
/// </list>
///
/// <para>This class is a <b>single-language</b> service. Use
/// <see cref="HuggingFaceOnnxNerServicePool"/> to combine EN + FR models under one
/// <see cref="INerService"/>.</para>
///
/// <para>Thread-safety: <see cref="InferenceSession"/> is thread-safe; this
/// class is safe to call concurrently as long as the injected tokenizer is too.</para>
/// </summary>
public sealed class HuggingFaceOnnxNerService : INerService, IDisposable
{
    private readonly InferenceSession _session;
    private readonly IHuggingFaceTokenizer _tokenizer;
    private readonly IReadOnlyList<string> _labels;
    private readonly string _language;
    private readonly string _inputIdsName;
    private readonly string _attentionMaskName;
    private readonly string? _tokenTypeIdsName;
    private readonly bool _ownsSession;

    /// <param name="session">Loaded ONNX <see cref="InferenceSession"/>.</param>
    /// <param name="tokenizer">Tokenizer matching the model (BERT vocab / CamemBERT SPM).</param>
    /// <param name="labels">Label id -&gt; label string map (e.g. ["O","B-PER","I-PER",...]).</param>
    /// <param name="language">BCP-47 code ("en", "fr") — used for wh-word / verb lexicons.</param>
    /// <param name="disposeSession">If true, <see cref="Dispose"/> also disposes the session.</param>
    public HuggingFaceOnnxNerService(
        InferenceSession session,
        IHuggingFaceTokenizer tokenizer,
        IReadOnlyList<string> labels,
        string language,
        bool disposeSession = false)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(tokenizer);
        ArgumentNullException.ThrowIfNull(labels);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        if (labels.Count == 0)
        {
            throw new ArgumentException("Label map must not be empty.", nameof(labels));
        }

        _session = session;
        _tokenizer = tokenizer;
        _labels = labels;
        _language = language;
        _ownsSession = disposeSession;

        // Auto-detect the model's input node names. We accept the common HF names.
        var inputNames = new HashSet<string>(_session.InputMetadata.Keys, StringComparer.Ordinal);
        _inputIdsName = FindInput(inputNames, "input_ids", "input_ids:0")
            ?? throw new InvalidOperationException("Model has no 'input_ids' input.");
        _attentionMaskName = FindInput(inputNames, "attention_mask", "attention_mask:0")
            ?? throw new InvalidOperationException("Model has no 'attention_mask' input.");
        _tokenTypeIdsName = FindInput(inputNames, "token_type_ids", "token_type_ids:0");
    }

    public Task<NerAnalysis> AnalyzeAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        cancellationToken.ThrowIfCancellationRequested();

        var encoded = _tokenizer.Encode(text);
        var seqLen = encoded.InputIds.Length;

        var inputs = new List<NamedOnnxValue>(3)
        {
            NamedOnnxValue.CreateFromTensor(
                _inputIdsName,
                new DenseTensor<long>(encoded.InputIds, [1, seqLen])),
            NamedOnnxValue.CreateFromTensor(
                _attentionMaskName,
                new DenseTensor<long>(encoded.AttentionMask, [1, seqLen])),
        };

        if (_tokenTypeIdsName is not null)
        {
            var typeIds = encoded.TokenTypeIds ?? new long[seqLen]; // default: zeros
            inputs.Add(NamedOnnxValue.CreateFromTensor(
                _tokenTypeIdsName,
                new DenseTensor<long>(typeIds, [1, seqLen])));
        }

        using var results = _session.Run(inputs);
        var logits = results[0].AsTensor<float>();       // [1, seq, numLabels]
        var numLabels = logits.Dimensions[2];

        var entities = AggregateBioSpans(text, encoded, logits, seqLen, numLabels);

        return Task.FromResult(new NerAnalysis(
            Lemma: null,
            Entities: entities,
            RootVerbs: RelationalVerbLexicon.Extract(text, _language),
            InterrogativeLemma: WhWordLexicon.Detect(text, _language)));
    }

    // -------------------------------------------------------------------------
    // BIO aggregation
    // -------------------------------------------------------------------------

    private IReadOnlyList<ExtractedEntity> AggregateBioSpans(
        string text,
        HuggingFaceEncoding encoded,
        Tensor<float> logits,
        int seqLen,
        int numLabels)
    {
        var entities = new List<ExtractedEntity>();

        string? currentTag = null;
        int currentStart = 0;
        int currentEnd = 0;
        var currentProbs = new List<double>();

        for (var i = 0; i < seqLen; i++)
        {
            if (encoded.AttentionMask[i] == 0)
            {
                Flush();
                continue;
            }

            var span = encoded.TokenCharSpans[i];
            if (span.IsSpecial)
            {
                Flush();
                continue;
            }

            // Argmax + softmax confidence for this token.
            var maxLogit = float.NegativeInfinity;
            var maxIndex = 0;
            for (var l = 0; l < numLabels; l++)
            {
                var v = logits[0, i, l];
                if (v > maxLogit)
                {
                    maxLogit = v;
                    maxIndex = l;
                }
            }

            double expSum = 0;
            for (var l = 0; l < numLabels; l++)
            {
                expSum += Math.Exp(logits[0, i, l] - maxLogit);
            }
            var prob = 1.0 / expSum;

            var label = maxIndex < _labels.Count ? _labels[maxIndex] : "O";
            var (prefix, tag) = ParseBioLabel(label);

            switch (prefix)
            {
                case 'O':
                    Flush();
                    break;

                case 'B':
                    Flush();
                    currentTag = tag;
                    currentStart = span.Start;
                    currentEnd = span.End;
                    currentProbs.Add(prob);
                    break;

                case 'I':
                    if (currentTag is not null && string.Equals(currentTag, tag, StringComparison.Ordinal))
                    {
                        currentEnd = span.End;
                        currentProbs.Add(prob);
                    }
                    else
                    {
                        // Orphan I-XXX: treat as a new B-XXX (common HF quirk).
                        Flush();
                        currentTag = tag;
                        currentStart = span.Start;
                        currentEnd = span.End;
                        currentProbs.Add(prob);
                    }
                    break;
            }
        }

        Flush();
        return entities;

        void Flush()
        {
            if (currentTag is null)
            {
                return;
            }

            var length = currentEnd - currentStart;
            if (length > 0)
            {
                var entityText = text.Substring(currentStart, length);
                var confidence = currentProbs.Count == 0 ? 1.0 : currentProbs.Average();
                entities.Add(new ExtractedEntity(
                    Text: entityText,
                    Type: currentTag,
                    Start: currentStart,
                    Length: length,
                    Confidence: confidence));
            }

            currentTag = null;
            currentStart = 0;
            currentEnd = 0;
            currentProbs.Clear();
        }
    }

    private static (char Prefix, string? Tag) ParseBioLabel(string label)
    {
        if (string.IsNullOrEmpty(label) || label == "O")
        {
            return ('O', null);
        }
        if (label.Length >= 2 && label[1] == '-' && (label[0] == 'B' || label[0] == 'I'))
        {
            return (label[0], label[2..]);
        }
        // Some models emit "PER" without BIO prefix — treat as begin.
        return ('B', label);
    }

    private static string? FindInput(HashSet<string> inputs, params string[] candidates)
    {
        foreach (var c in candidates)
        {
            if (inputs.Contains(c))
            {
                return c;
            }
        }
        return null;
    }

    public void Dispose()
    {
        if (_ownsSession)
        {
            _session.Dispose();
        }
    }
}
