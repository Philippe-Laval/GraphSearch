namespace GraphSearch.Library.Embeddings
{
    public class OllamaEmbeddingService : IEmbeddingService
    {
        public Task<ReadOnlyMemory<float>> EmbedAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
