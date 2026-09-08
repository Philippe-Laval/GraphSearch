using GraphSearch.Library.Query.Resolution;

namespace GraphSearch.Library.Query.Analysis.Model;

/// <summary>
/// Represents the analysis of a query, 
/// including its original and normalized forms, 
/// extracted entities, resolved entities, and embedding.
/// </summary>
public sealed class QueryAnalysis
{
    /// <summary>
    /// Original query submitted by the user
    /// </summary>
    public required string OriginalQuery { get; init; }

    /// <summary>
    /// Normalized query
    /// </summary>
    public required string NormalizedQuery { get; init; }

    /// <summary>
    /// Extracted entities from the query, including their text, type, position, length, and confidence score.
    /// </summary>
    public required IReadOnlyList<ExtractedEntity> Entities { get; init; }

    /// <summary>
    /// Resolved entities from the query
    /// </summary>
    public required IReadOnlyList<ResolvedEntity> ResolvedEntities { get; init; }

    /// <summary>
    /// Embedding of normalized query.
    /// </summary>
    // ReadOnlyMemory<float> is a good choice for embeddings in .NET
    // because most embedding APIs return something compatible with it
    // and it avoids unnecessary allocations/copies.
    public required ReadOnlyMemory<float> Embedding { get; init; }

    /// <summary>
    /// Inferred intent of the query. Defaults to <see cref="QueryIntent.Unknown"/>
    /// so older analyzers that only produce normalized text still populate a
    /// valid <see cref="QueryAnalysis"/>.
    /// </summary>
    public QueryIntent Intent { get; init; } = QueryIntent.Unknown;

    /// <summary>
    /// Confidence in the inferred <see cref="Intent"/>, in [0, 1].
    /// </summary>
    public double IntentConfidence { get; init; } = 1.0;

    /// <summary>
    /// Alternative phrasings of the query (produced by <see cref="QueryRewritingAnalyzer"/>
    /// or LLM-based analyzers). Feed these into hybrid retrieval alongside
    /// <see cref="NormalizedQuery"/>.
    /// </summary>
    public IReadOnlyList<string> Rewrites { get; init; } = [];

    /// <summary>
    /// Detected BCP-47 language code (e.g. "en", "fr"). Null when unknown.
    /// </summary>
    public string? DetectedLanguage { get; init; }

    /// <summary>
    /// Logical graph pattern (anchor entities, node/relationship types) suitable
    /// for driving a graph query planner. Null when the analyzer did not
    /// produce one.
    /// </summary>
    public GraphQueryPattern? GraphPattern { get; init; }
}
