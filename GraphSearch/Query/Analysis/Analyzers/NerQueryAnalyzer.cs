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

    /// <summary>
    /// Initialise une nouvelle instance de la classe <c>NerQueryAnalyzer</c> avec un service de reconnaissance
    /// d’entités nommées et, en option, un analyseur de normalisation de requête.
    /// </summary>
    /// <param name="ner">Service de reconnaissance d’entités nommées utilisé pour analyser la requête.</param>
    /// <param name="normalizer">Analyseur appliqué avant l’analyse NER. Si <see langword="null" />, une instance de
    /// <c>RulesBasedQueryAnalyzer</c> est utilisée avec <c>DetectIntent</c> défini sur <see langword="false" />.</param>
    public NerQueryAnalyzer(INerService ner, IQueryAnalyzer? normalizer = null)
    {
        ArgumentNullException.ThrowIfNull(ner);

        _ner = ner;
        _normalizer = normalizer ?? new RulesBasedQueryAnalyzer(QueryAnalyzerOptions.Default with
        {
            // Disable intent detection in the normalizer, since we will classify intent ourselves.
            DetectIntent = false,
        });
    }

    /// <inheritdoc/>
    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // First normalize the query
        var baseAnalysis = await _normalizer.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);

        // Then run NER on the normalized form.
        NerAnalysis ner = await _ner.AnalyzeAsync(baseAnalysis.NormalizedQuery, cancellationToken).ConfigureAwait(false);

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

    /// <summary>
    /// Classe l’intention d’une requête à partir du mot interrogatif détecté, du nombre de verbes racines et du nombre
    /// d’entités.
    /// </summary>
    /// <param name="wh">Mot interrogatif normalisé détecté dans la requête, ou <see langword="null"/> s’il est absent.</param>
    /// <param name="rootVerbs">Nombre de verbes racines identifiés dans la requête.</param>
    /// <param name="entities">Nombre d’entités identifiées dans la requête.</param>
    /// <returns>Valeur <see cref="QueryIntent"/> correspondant au type de requête le plus probable selon les signaux
    /// linguistiques fournis.</returns>
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
