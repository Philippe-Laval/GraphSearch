namespace GraphRag.Lucene.Models;

/// <summary>
/// ChunkDocument represents a chunk of text from a document,
/// along with its associated metadata and entity identifiers.
/// </summary>
/// <param name="Id">The chunk identifier.</param>
/// <param name="DocumentId">The identifier of the document this chunk belongs to.</param>
/// <param name="Text">The text content of the chunk.</param>
/// <param name="EntityIds">The identifiers of entities associated with the chunk.</param>
/// <param name="Metadata">Optional metadata associated with the chunk.</param>
public sealed record ChunkDocument(
    int Id,
    int DocumentId,
    string Text,
    string[] EntityIds,
    Dictionary<string, string>? Metadata = null);