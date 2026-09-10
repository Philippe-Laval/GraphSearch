# Entity Extractor

## First candidate extractors

1.	EntitySpanMerger — shared helper for overlap/dedup/priority merging (used by hybrid + others).
2.	RegexEntityExtractor + RegexEntityRule — pattern rules (dates, versions, URLs, tickers…).
3.	NerEntityExtractor — wraps any INerService (spaCy / Azure / HF-ONNX).
4.	LlmEntityExtractor — prompt-based, JSON output, tolerant parser.
5.	HybridEntityExtractor — priority + confidence-weighted merge across multiple extractors.
6.	CachingEntityExtractor — decorator, concurrent dict.
7.  DictionaryEntityExtractor


| File  | Role  |
| :------ | :------ |
| EntitySpanMerger.cs  | Shared overlap/priority/confidence merge helper  |
| RegexEntityExtractor.cs + RegexEntityRule  | Rule-based (versions, dates, URLs, emails, custom)  |
| NerEntityExtractor.cs  | Wraps any INerService (spaCy / Azure / HF-ONNX)  |
| LlmEntityExtractor.cs  | Prompt → JSON array → re-locate spans in text  |
| HybridEntityExtractor.cs  | Priority-ordered composition + resilient exception handling  |
| CachingEntityExtractor.cs  | Concurrent decorator for expensive backends  |
| DictionaryEntityExtractor.cs  | Dictionary-based entity extraction  |

Recommended GraphRAG stack, wired via HybridEntityExtractor (first = highest priority):

```
IEntityExtractor extractor = new CachingEntityExtractor(
    new HybridEntityExtractor(
        DictionaryEntityExtractorFactory.Create(),   // curated gazetteer   (P=4)
        RegexEntityExtractor.CreateDefault(),        // structured tokens   (P=3)
        new NerEntityExtractor(nerService),          // open-vocabulary NER (P=2)
        new LlmEntityExtractor(chatClient)));        // last-mile recall    (P=1)
```

## Other extractor strategies worth considering

Not implemented, but sensible next additions:
1.	AhoCorasickGazetteerExtractor — trie-based multi-pattern matcher; O(n + matches) vs the current dictionary's O(entities × n). Worth adding once your entity vocabulary exceeds ~10 k.
2.	FuzzyEntityExtractor — Levenshtein/Jaro-Winkler wrapper for typo tolerance ("Micosoft" → Microsoft). Best combined with a dictionary or gazetteer.
3.	EmbeddingCandidateEntityExtractor — encode noun-phrase spans, cosine-match against a vector index of known entities. Powerful for semantic aliases ("the Redmond company" → Microsoft).
4.	PhraseChunkEntityExtractor — use spaCy noun-chunk output as candidate mentions before typing them (great fallback when NER labels are sparse).
5.	GraphResolvableEntityFilter — decorator restricting output to entities that exist in your knowledge graph vocabulary (deduplicates with the downstream EntityResolver).
6.	SpanClassificationEntityExtractor — ONNX universal-NER model (e.g. nuner) for zero-shot typed spans; complements task-specific NER.
7.	Metric/telemetry decorator — TelemetryEntityExtractor recording latency, entity counts, and cache hit ratio per tier — very useful once you're comparing tiers in production.


| File  | What it does  |
| :------ | :------ |
| AhoCorasickGazetteerExtractor.cs  | Trie-based multi-pattern matcher — O(n + matches) after an O(Σ\|patterns\|) build. Case-insensitive patterns, preserves original casing in output.  |
| FuzzyEntityExtractor.cs  | Sliding token-window (1..4) + bounded Levenshtein with early row-min exit. Normalized-distance threshold, min-token-length guard against short-string noise. |
| EmbeddingCandidateEntityExtractor.cs  | Encode noun-phrase spans, cosine-match against a vector index of known entities. Powerful for semantic aliases ("the Redmond company" → Microsoft). |
| IPhraseChunkService.cs + PhraseChunk record | Abstraction for noun-chunk / phrase-candidate producers (spaCy, custom chunker, regex fallback). |
| RegexPhraseChunkService.cs | Dependency-free heuristic chunker: capitalized-word runs joined by function words (of, de, du, la, the, …). Fine as fallback; swap for a spaCy-backed impl when the microservice is available. |
| PhraseChunkEntityExtractor.cs | Emits chunks as candidates with type "NounPhrase" (configurable). Low-priority recall booster. |
| EmbeddingCandidateEntityExtractor.cs + EmbeddedEntity record + BuildIndexAsync helper | Uses IEmbeddingService + IPhraseChunkService. Cosine-matches candidates against a pre-embedded index (in-memory). |
| GraphResolvableEntityFilter.cs + IGraphVocabulary + SetGraphVocabulary | Decorator dropping entities absent from the graph vocabulary. SetGraphVocabulary is the in-memory default. |
| SpanClassificationEntityExtractor.cs | Adapter over any INerService (e.g. an ONNX universal-NER model). Type-remapping, allow-list, min-confidence filter. Complements task-specific NER without duplicating ONNX plumbing. |


Recommended end-to-end composition:

```csharp
var vocab = new SetGraphVocabulary(graph.EntityNames);
var chunker = new RegexPhraseChunkService();
var entityIndex = await EmbeddingCandidateEntityExtractor.BuildIndexAsync(
    graph.EntityDefinitions, embeddingService);

IEntityExtractor extractor = new CachingEntityExtractor(
    new GraphResolvableEntityFilter(
        new HybridEntityExtractor(
            DictionaryEntityExtractorFactory.Create(),                   // P=6 exact curated
            new AhoCorasickGazetteerExtractor(graph.EntityDefinitions),  // P=5 exact large gazetteer
            RegexEntityExtractor.CreateDefault(),                        // P=4 structured tokens
            new NerEntityExtractor(taskSpecificNer),                     // P=3 PER/ORG/LOC
            new SpanClassificationEntityExtractor(universalNer,          // P=2 zero-shot
                typeMap: new Dictionary<string,string> {["company"]="Organization"}),
            new EmbeddingCandidateEntityExtractor(chunker, embeddings, entityIndex), // P=1 aliases
            new FuzzyEntityExtractor(graph.EntityDefinitions),           // P=1 typos
            new PhraseChunkEntityExtractor(chunker),                     // P=0 last-resort NP candidates
            new LlmEntityExtractor(chatClient)                           // P=0 LLM safety net
        ),
        vocab));
```

Design notes worth calling out:
-	Aho–Corasick node storage uses per-node Dictionary<char,int> — sparse and safe for arbitrary Unicode. If you regularly deal with >1M patterns and hot inner loops, swap to a compact sorted-array + binary-search layout for better cache behaviour.
-	Fuzzy extractor intentionally skips exact-match hits (distance == 0) — those belong to the dictionary/gazetteer tier, so it stays a pure typo-tolerance layer.
-	Embedding extractor is O(candidates × entities) in-memory. Above ~100k entities, swap the linear scan for an ANN backend (Chroma is already referenced in Query/Resolution, so a ChromaGraphVocabulary would be a natural extension).
-	PhraseChunkEntityExtractor placed at the lowest priority so its untyped "NounPhrase" output never shadows a real NER label on overlap.
-	GraphResolvableEntityFilter is placed outside the composite (not as a member) so all extractors get to vote before the graph guardrail runs — this preserves the priority/merge semantics of HybridEntityExtractor and avoids double-filtering.
-	SpanClassificationEntityExtractor is deliberately an adapter over INerService rather than a fresh ONNX runtime, so the existing HuggingFaceOnnxNerServicePool can host both task-specific and universal models with zero duplication.

