using Azure;
using GraphRag.AI.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.AI.EmbeddingServices
{
    public class OpenAIEmbeddingService : IEmbeddingService, IDisposable
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _generator;

        public OpenAIEmbeddingService(IOptions<OpenAiOptions> aiOptions)
        {
            OpenAiOptions options = aiOptions.Value;

            ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.EmbeddingModel);

            OpenAIClient client = options.Endpoint is null
                ? new OpenAIClient(options.ApiKey)
                : new OpenAIClient(
                    new ApiKeyCredential(options.ApiKey),
                    new OpenAIClientOptions { Endpoint = options.Endpoint });

            _generator = client
                .GetEmbeddingClient(options.EmbeddingModel)
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
