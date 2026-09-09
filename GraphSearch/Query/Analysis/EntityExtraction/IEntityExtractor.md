

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

