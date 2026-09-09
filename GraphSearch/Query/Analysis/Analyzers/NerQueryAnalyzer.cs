namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Analyzer driven by an external NER pipeline. Uses part-of-speech / dependency
/// cues (root verbs, wh-word, entity count) to classify intent and produce a
/// richer normalized form (lemmatized text if available).
///
/// Rules of thumb:
///   wh-word "how many" / "count"        -> Aggregation
///   wh-word "why" / "how does"          -> Explanation
///   2+ root verbs + 2+ entities         -> MultiHopRelationship
///   1 root verb + 2 entities            -> Relationship
///   wh-word "what"/"who" + 1 entity     -> EntityLookup
/// </summary>
public sealed class NerQueryAnalyzer : IQueryAnalyzer
{
    private static readonly HashSet<string> AggregationWh =
        new(StringComparer.OrdinalIgnoreCase) { "how many", "how much", "count" };

    private static readonly HashSet<string> ExplanationWh =
        new(StringComparer.OrdinalIgnoreCase) { "why", "how", "how does", "how do" };

    private static readonly HashSet<string> LookupWh =
        new(StringComparer.OrdinalIgnoreCase) { "what", "who", "which" };

    private readonly INerService _ner;
    private readonly IQueryAnalyzer _normalizer;

    public NerQueryAnalyzer(INerService ner, IQueryAnalyzer? normalizer = null)
    {
        ArgumentNullException.ThrowIfNull(ner);
        _ner = ner;
        _normalizer = normalizer ?? new RulesBasedQueryAnalyzer(QueryAnalyzerOptions.Default with
        {
            DetectIntent = false,
        });
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var baseAnalysis = await _normalizer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);
        var ner = await _ner.AnalyzeAsync(baseAnalysis.NormalizedQuery, cancellationToken).ConfigureAwait(false);

        var wh = ner.InterrogativeLemma;
        var intent = ClassifyIntent(wh, ner.RootVerbs.Count, ner.Entities.Count);

        var normalized = !string.IsNullOrWhiteSpace(ner.Lemma)
            ? ner.Lemma!.Trim()
            : baseAnalysis.NormalizedQuery;

        return baseAnalysis with
        {
            NormalizedQuery = normalized,
            Intent = intent,
            IntentConfidence = intent == QueryIntent.Unknown ? 0.0 : 0.75,
        };
    }

    private static QueryIntent ClassifyIntent(string? wh, int rootVerbs, int entities)
    {
        if (wh is not null && AggregationWh.Contains(wh))
        {
            return QueryIntent.Aggregation;
        }

        if (wh is not null && ExplanationWh.Contains(wh))
        {
            return QueryIntent.Explanation;
        }

        if (rootVerbs >= 2 && entities >= 2)
        {
            return QueryIntent.MultiHopRelationship;
        }

        if (rootVerbs >= 1 && entities >= 2)
        {
            return QueryIntent.Relationship;
        }

        if (wh is not null && LookupWh.Contains(wh) && entities >= 1)
        {
            return QueryIntent.EntityLookup;
        }

        return entities >= 1 ? QueryIntent.General : QueryIntent.Unknown;
    }
}
