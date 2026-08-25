using GraphSearch.Query.Resolution;

namespace GraphSearch.Query.Analysis;

public sealed class QueryAnalysis
{
    public required string OriginalQuery { get; init; }

    public required string NormalizedQuery { get; init; }

    public required IReadOnlyList<ExtractedEntity> Entities { get; init; }

    public required IReadOnlyList<ResolvedEntity> ResolvedEntities { get; init; }

    /// <summary>
    /// Embedding of normalized query
    /// </summary>
    // ReadOnlyMemory<float> is a good choice for embeddings in .NET
    // because most embedding APIs return something compatible with it
    // and it avoids unnecessary allocations/copies.
    public required ReadOnlyMemory<float> Embedding { get; init; }
}

