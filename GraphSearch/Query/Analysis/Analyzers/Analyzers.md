# Analysis of analyzers

Service abstractions (implement to plug into your infra)
-	IChatCompletionClient — LLM (OpenAI / Azure OpenAI / Ollama / …).
-	INerService + NerAnalysis — external NER (spaCy / HF / Azure Language).
-	ILanguageDetector + HeuristicLanguageDetector (stopword-based EN/FR baseline).
-	IQueryRewriter + SynonymQueryRewriter (regex-driven baseline).

The 8 analyzers

| #  | Class                                                | Notes  |
| :--| :--------------------------------------------------- | :------ |
| 1  | 	LlmQueryAnalyzer                                    | Prompts an IChatCompletionClient, parses {normalizedQuery, intent, rewrites, confidence}; robust to  json   fences. Wrap with CachingQueryAnalyzer. |
| 2  | 	EmbeddingIntentClassifier + EmbeddingQueryAnalyzer  | Trainable prototypes (mean-of-examples, L2-normalized). Analyzer normalizes → embeds → cosine-classifies; stackalloc for small vectors. |
| 3  | 	NerQueryAnalyzer                                    | Uses wh-word / root-verb / entity counts from NerAnalysis to classify intent; substitutes lemma if provided. |
| 4  | 	RegexRuleQueryAnalyzer + IntentRule                 | Prioritized regex rules; JSON loader (FromJson) for hot-reload without recompiling. |
| 5  | 	CompositeQueryAnalyzer                              | Chains analyzers; default acceptance predicate is "not Unknown/General". Ideal for rules → embedding → LLM fallback. |
| 6  | 	QueryRewritingAnalyzer                              | Decorator — merges rewrites from IQueryRewriter into AnalyzedQuery.Rewrites. |
| 7  | 	MultilingualQueryAnalyzer                           | Routes on detected language; sets DetectedLanguage; falls back to default analyzer. |
| 8  | 	GraphPatternQueryAnalyzer                           | Builds GraphQueryPattern from IEntityExtractor + RelationshipVocabulary; upgrades intent to Relationship / MultiHopRelationship based on entity+relationship counts. |

Wiring examples

```csharp

// Cheap-first → LLM fallback, cached, rewrites enabled.
IQueryAnalyzer analyzer = new CachingQueryAnalyzer(
    new QueryRewritingAnalyzer(
        new CompositeQueryAnalyzer(
            new QueryAnalyzer(),                                    // rules
            new EmbeddingQueryAnalyzer(embed, classifier),          // paraphrase
            new LlmQueryAnalyzer(chatClient)),                      // hard cases
        new SynonymQueryRewriter()
            .Add("microsoft's tech", "technologies used by microsoft")));

// French + English routing.
IQueryAnalyzer multi = new MultilingualQueryAnalyzer(
    new HeuristicLanguageDetector(),
    new Dictionary<string, IQueryAnalyzer>
    {
        ["en"] = new QueryAnalyzer(),
        ["fr"] = new QueryAnalyzer(QueryAnalyzerOptions.Default with { RemoveDiacritics = true }),
    },
    defaultAnalyzer: new QueryAnalyzer());

// Emit a logical graph pattern for the planner.
IQueryAnalyzer planner = new GraphPatternQueryAnalyzer(
    inner: new QueryAnalyzer(),
    entityExtractor: myEntityExtractor,
    vocabulary: myRelationshipVocabulary,
    relationshipCandidates: ["develops", "runs-on", "owns", "located-in"]);

```