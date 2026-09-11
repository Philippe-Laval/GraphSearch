using GraphRag.Core.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace GraphSearch.Library.Embeddings
{
    public class OllamaEmbeddingService : IEmbeddingService, IDisposable
    {
        private readonly OllamaApiClient _client;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

        public OllamaEmbeddingService(IOptions<OllamaOptions> aiOptions)
        {
            OllamaOptions options = aiOptions.Value;

            ArgumentNullException.ThrowIfNull(options.Endpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.EmbeddingModel);

            _client = new OllamaApiClient(options.Endpoint, options.EmbeddingModel);
            _generator = _client;
        }

        public async Task<ReadOnlyMemory<float>> EmbedAsync(string text, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);

            Embedding<float> embedding = await _generator
                .GenerateAsync(text, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return embedding.Vector;
        }

        public void Dispose() => _client.Dispose();
    }
}