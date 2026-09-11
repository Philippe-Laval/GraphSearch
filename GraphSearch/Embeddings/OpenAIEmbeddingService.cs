using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSearch.Library.Embeddings
{
    public class OpenAIEmbeddingService : IEmbeddingService
    {
        private readonly string _apiKey;

        public OpenAIEmbeddingService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Task<ReadOnlyMemory<float>> EmbedAsync(string text, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
