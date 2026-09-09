using GraphSearch.Library.EdgeScoring;

namespace GraphSearch.Library.Query.Analysis.Analyzers;

/// <summary>
/// Decorator that augments an <see cref="AnalyzedQuery"/> with a logical
/// <see cref="GraphQueryPattern"/>: anchor entities (from
/// <see cref="IEntityExtractor"/>) plus canonical relationship types (matched
/// via a <see cref="RelationshipVocabulary"/>).
///
/// This is the entry point for driving Binder / LogicalOperator construction
/// from natural language, instead of relying purely on vector search.
///
/// Example — "Which products developed by Microsoft run on Linux?":
///   AnchorEntities    = ["Microsoft", "Linux"]
///   RelationshipTypes = ["develops", "runs-on"]
///   NodeTypes         = [] (populate via a domain-specific extension)
/// </summary>
public sealed class GraphPatternQueryAnalyzer : IQueryAnalyzer
{
    private readonly IQueryAnalyzer _inner;
    private readonly IEntityExtractor _entityExtractor;
    private readonly IReadOnlyList<string> _relationshipCandidates;
    private readonly RelationshipVocabulary _vocabulary;
    private readonly double _relationshipThreshold;

    /// <param name="inner">Inner analyzer providing normalization + intent.</param>
    /// <param name="entityExtractor">Extractor for anchor entities.</param>
    /// <param name="vocabulary">Vocabulary mapping surface forms to canonical relationships.</param>
    /// <param name="relationshipCandidates">
    /// Canonical relationship names to test against the query text
    /// (e.g. ["develops", "runs-on", "owns", "located-in"]).
    /// </param>
    /// <param name="relationshipThreshold">
    /// Minimum <see cref="RelationshipVocabulary.Similarity"/> to accept a
    /// relationship. Default 0.5.
    /// </param>
    public GraphPatternQueryAnalyzer(
        IQueryAnalyzer inner,
        IEntityExtractor entityExtractor,
        RelationshipVocabulary vocabulary,
        IEnumerable<string> relationshipCandidates,
        double relationshipThreshold = 0.5)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(entityExtractor);
        ArgumentNullException.ThrowIfNull(vocabulary);
        ArgumentNullException.ThrowIfNull(relationshipCandidates);

        _inner = inner;
        _entityExtractor = entityExtractor;
        _vocabulary = vocabulary;
        _relationshipCandidates = relationshipCandidates.ToList();
        _relationshipThreshold = relationshipThreshold;
    }

    public async Task<AnalyzedQuery> AnalyzeAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        // Run the inner analyzer first to get the normalized query and any intent it may have produced.
        var analysis = await _inner.AnalyzeAsync(query, cancellationToken).ConfigureAwait(false);

        // Extract entities from the normalized query.
        var entities = await _entityExtractor
            .ExtractAsync(analysis.NormalizedQuery, cancellationToken)
            .ConfigureAwait(false);

        // Deduplicate and normalize anchor entities.
        var anchors = entities
            .Select(e => e.Text)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Tokenize normalized query into words to test relationship similarity.
        var words = analysis.NormalizedQuery
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        var matchedRelationships = new List<string>();
        foreach (var canonical in _relationshipCandidates)
        {
            var hit = false;
            foreach (var word in words)
            {
                if (_vocabulary.Similarity(word, canonical) >= _relationshipThreshold)
                {
                    hit = true;
                    break;
                }
            }
            if (hit && !matchedRelationships.Contains(canonical, StringComparer.OrdinalIgnoreCase))
            {
                matchedRelationships.Add(canonical);
            }
        }

        // Upgrade intent when we see multiple relationships + entities.
        var intent = analysis.Intent;
        if (matchedRelationships.Count >= 2 && anchors.Count >= 2)
        {
            intent = QueryIntent.MultiHopRelationship;
        }
        else if (matchedRelationships.Count == 1 && anchors.Count >= 2)
        {
            intent = intent is QueryIntent.Unknown or QueryIntent.General
                ? QueryIntent.Relationship
                : intent;
        }

        var pattern = new GraphQueryPattern(
            AnchorEntities: anchors,
            NodeTypes: Array.Empty<string>(),
            RelationshipTypes: matchedRelationships);

        return analysis with
        {
            Intent = intent,
            GraphPattern = pattern,
        };
    }
}
