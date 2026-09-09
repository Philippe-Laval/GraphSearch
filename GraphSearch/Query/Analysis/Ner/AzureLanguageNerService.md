# AzureLanguageNerService

Azure AI Language gives us entities and language detection but no POS tagging or lemmatization, so I'll:
1.	Use Azure.AI.TextAnalytics for entities (EN + FR supported).
2.	Detect the wh-word in C# using a small WhWordLexicon (same normalization strategy as the spaCy service — FR words map to canonical EN lemmas).
3.	Approximate root verbs with a RelationalVerbLexicon (dictionary of common relational verbs in EN + FR).
4.	Return Lemma = null (Azure doesn't expose one); NerQueryAnalyzer already falls back to the normalized query in that case.

New pieces
-	NuGet reference: Azure.AI.TextAnalytics 5.3.0 added to GraphSearch.Library.csproj.
-	WhWordLexicon (static) — detects wh-word in EN/FR text and returns the canonical English lemma ("combien" → "how many", "pourquoi" → "why", "quel"/"quelle"/... → "which", etc.). Handles multi-word cues (how many, combien de, comment est, qu' est-ce que-tolerant, missing accents like ou/developpe).
-	RelationalVerbLexicon (static) — dictionary of ~150 EN/FR verb forms mapped to canonical English lemmas (développé → develop, fonctionne → run, possède → own, ...). Returns deduplicated verbs in insertion order.
-	AzureLanguageNerService : INerService — uses TextAnalyticsClient.RecognizeEntitiesAsync for entities (with ConfidenceScore), then locally derives RootVerbs via the verb lexicon and InterrogativeLemma via the wh-word lexicon. Lemma = null (Azure doesn't provide one); NerQueryAnalyzer already falls back to the normalized query in that case.
-	Language selection order: explicit argument → ILanguageDetector → configured default. Regional variants like en-US / fr-CA are stripped to Azure's base code.

Wire-up

```
var client = new TextAnalyticsClient(
    new Uri("https://<your-resource>.cognitiveservices.azure.com/"),
    new AzureKeyCredential("<KEY>"));

INerService ner = new AzureLanguageNerService(
    client,
    languageDetector: new HeuristicLanguageDetector(),
    defaultLanguage: "en");

IQueryAnalyzer analyzer = new NerQueryAnalyzer(ner);
```


Trade-offs vs. the spaCy microservice

| Capability  | spaCy service  | Azure AI Language  |
| :------ | :------ | :------ |
| Entities  | ✅ (offline)  | ✅ (higher recall, calibrated conf.)  |
| Language detection  | ✅ via spaCy pipeline  | ✅ built-in  |
| Lemmatization  | ✅ full sentence  | ❌ (returns null)  |
| POS tagging  | ✅  | ❌  |
| Root-verb detection  | ✅ real POS  | ⚠️ dictionary approximation  |
| Wh-word detection  | ✅ POS + multi-word  | ⚠️ lexicon-based (same table)  |
| Hosting  | Self-hosted Python  | Managed Azure service  |
| Cost  | Free (compute only)  | Per-call pricing  |

For production, NerQueryAnalyzer still works with either backend unchanged — you can even wrap both in a CompositeQueryAnalyzer (Azure primary, spaCy fallback, or vice versa).

Small design note first: CompositeQueryAnalyzer currently doesn't fall back if an inner analyzer throws (network glitch, auth failure). For an Azure↔spaCy composition, that resilience matters, so I'll:
1.	Add an opt-in continueOnException param to CompositeQueryAnalyzer (backwards compatible).
2.	Add a HybridNerAnalyzers static factory with AzurePrimary(...) and SpacyPrimary(...) helpers that wire both NER services into a resilient composite.

CompositeQueryAnalyzer — extended (backwards compatible)
-	New optional constructor params:
-	continueOnException: bool = false — when true, an inner analyzer that throws is skipped instead of failing the pipeline.
-	onException: Action<Exception, IQueryAnalyzer>? = null — hook for logging.
-	OperationCanceledException triggered by the caller's token is always re-thrown.
-	If every inner analyzer fails when continueOnException is on, throws InvalidOperationException wrapping the last failure.
HybridNerAnalyzers (new static factory)

Two one-liners to build a resilient Azure↔spaCy pipeline:

```
// Azure primary, spaCy fallback (recommended when Azure is your main path)
IQueryAnalyzer analyzer = new CachingQueryAnalyzer(
    HybridNerAnalyzers.AzurePrimary(
        azure: new AzureLanguageNerService(textAnalyticsClient, langDetector),
        spacy: new SpacyNerService(spacyHttpClient, langDetector),
        onException: (ex, inner) => logger.LogWarning(ex, "{Analyzer} failed", inner.GetType().Name)));

// spaCy primary, Azure fallback (privacy/cost-optimized default, cloud escalation)
IQueryAnalyzer analyzer = new CachingQueryAnalyzer(
    HybridNerAnalyzers.SpacyPrimary(
        spacy: new SpacyNerService(spacyHttpClient, langDetector),
        azure: new AzureLanguageNerService(textAnalyticsClient, langDetector)));
```

Fallback semantics

The composite moves to the secondary analyzer when the primary either:
-	returns QueryIntent.Unknown or QueryIntent.General (weak/absent classification), or
-	throws — Azure quota / auth / network failure ⇒ instantly falls through to spaCy (and vice versa).

You can plug either factory anywhere an IQueryAnalyzer is expected — including inside a larger CompositeQueryAnalyzer chain (e.g. rules → hybrid-NER → LLM), or as the inner of QueryRewritingAnalyzer / MultilingualQueryAnalyzer / GraphPatternQueryAnalyzer.


