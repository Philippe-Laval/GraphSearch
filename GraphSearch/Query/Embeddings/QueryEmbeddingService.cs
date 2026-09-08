namespace GraphSearch.Library.Query.Embeddings;

// The important point is that your GraphRAG code doesn't know whether
// you're using OpenAI, Azure OpenAI, Ollama, a local model, etc.

public sealed class QueryEmbeddingService
{
    private readonly IEmbeddingService _embeddingService;

    public QueryEmbeddingService(
        IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public Task<ReadOnlyMemory<float>> CreateAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        return _embeddingService.EmbedAsync(
            query,
            cancellationToken);
    }
}