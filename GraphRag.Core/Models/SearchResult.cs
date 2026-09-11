namespace GraphRag.Core.Models;

/// <summary>
/// Represents a search result with a chunk ID and a relevance score.
/// </summary>
public sealed record SearchResult(
    int ChunkId,
    double Score);