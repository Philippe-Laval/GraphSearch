namespace GraphRag.Lucene.Models;

/// <summary>
/// SourceDocument represents a document that is stored in the source repository 
/// and is queued for processing into chunks for indexing in Lucene.
/// </summary>
/// <param name="Id">The unique identifier of the document.</param>
/// <param name="RelativePath">The relative path of the document within the repository.</param>
/// <param name="ContentHash">The hash of the document's content, used to detect changes.</param>
/// <param name="LastWriteTimeUtc">The last write time of the document in UTC.</param>
/// <param name="Metadata">Optional metadata associated with the document.</param>
public sealed record SourceDocument(
    int Id,
    string RelativePath,
    string ContentHash,
    DateTime LastWriteTimeUtc,
    Dictionary<string, string>? Metadata = null);