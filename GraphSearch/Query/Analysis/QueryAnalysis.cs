using GraphSearch.Library.Query.Resolution;

namespace GraphSearch.Library.Query.Analysis;

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
}

