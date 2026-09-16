namespace GraphRag.AI.EmbeddingServices;

public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> EmbedAsync(
        string text,
        CancellationToken cancellationToken = default);
}