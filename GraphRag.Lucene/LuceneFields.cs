namespace GraphRag.Lucene;

/// <summary>
/// Lucene fields used for indexing and searching chunk documents.
/// </summary>
public static class LuceneFields
{
    public const string ChunkId = "chunk_id";
    public const string DocumentId = "document_id";
    public const string Text = "text";
    public const string EntityId = "entity_id";
    public const string MetadataPrefix = "meta_";
}

