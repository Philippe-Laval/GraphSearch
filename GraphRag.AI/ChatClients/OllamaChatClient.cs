using GraphRag.AI.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace GraphRag.AI.ChatClients
{
    public class OllamaChatClient : IChatClient, IDisposable
    {
        private readonly IChatClient _chatClient;

        public OllamaChatClient(IOptions<OllamaOptions> aiOptions)
        {
            OllamaOptions options = aiOptions.Value;

            ArgumentNullException.ThrowIfNull(options.Endpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ChatModel);

            var client = new OllamaApiClient(options.Endpoint, options.ChatModel);
            _chatClient = (IChatClient)client;
        }

        public OllamaChatClient(OllamaOptions options)
        {
            ArgumentNullException.ThrowIfNull(options.Endpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ChatModel);

            var client = new OllamaApiClient(options.Endpoint, options.ChatModel);
            _chatClient = (IChatClient)client;
        }

        public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
            => _chatClient.GetResponseAsync(messages, options, cancellationToken);

        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
            => _chatClient.GetStreamingResponseAsync(messages, options, cancellationToken);

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            if (serviceType.IsInstanceOfType(this))
                return this;

            return _chatClient.GetService(serviceType, serviceKey);
        }

        public void Dispose() => _chatClient.Dispose();
    }
}
