namespace GraphSearch.Query.Embeddings;

public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> EmbedAsync(
        string text,
        CancellationToken cancellationToken = default);
}