using Azure;
using Azure.AI.OpenAI;
using GraphRag.Core.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace GraphSearch.Library.Embeddings
{
    public class AzureOpenAIEmbeddingService : IEmbeddingService, IDisposable
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

        public AzureOpenAIEmbeddingService(IOptions<AzureOpenAiOptions> aiOptions)
        {
            AzureOpenAiOptions options = aiOptions.Value;

            ArgumentNullException.ThrowIfNull(options.Endpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.EmbeddingDeploymentName);

            _generator = new AzureOpenAIClient(
                    options.Endpoint,
                    new AzureKeyCredential(options.ApiKey))
                .GetEmbeddingClient(options.EmbeddingDeploymentName)
                .AsIEmbeddingGenerator();
        }

        public async Task<ReadOnlyMemory<float>> EmbedAsync(string text, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);

            Embedding<float> embedding = await _generator
                .GenerateAsync(text, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return embedding.Vector;
        }

        public void Dispose() => _generator.Dispose();
    }
}