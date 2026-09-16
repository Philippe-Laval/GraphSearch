using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace GraphRag.NLP.Tests
{
    /// <summary>
    /// Client de chat simulé pour les tests.
    /// </summary>
    internal sealed class FakeChatClient : IChatClient
    {
        private readonly string _response;

        public FakeChatClient(string response)
        {
            _response = response;
        }

        public ChatClientMetadata Metadata { get; } =
            new(providerName: "FakeChatClient", providerUri: null, defaultModelId: "fake-model");

        public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new ChatResponse(
                    new ChatMessage(ChatRole.Assistant, _response)));
        }

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ChatResponse response = await GetResponseAsync(messages, options, cancellationToken);

            foreach (ChatMessage message in response.Messages)
            {
                yield return new ChatResponseUpdate
                {
                    Role = message.Role,
                    Contents = message.Contents
                };
            }
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            return serviceKey is null && serviceType.IsInstanceOfType(this)
                ? this
                : null;
        }

        public void Dispose()
        {
        }
    }
}
