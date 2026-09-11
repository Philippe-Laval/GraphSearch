namespace GraphSearch.Library.Embeddings
{
    public class AzureOpenAIEmbeddingService : IEmbeddingService
    {
        public Task<ReadOnlyMemory<float>> EmbedAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
