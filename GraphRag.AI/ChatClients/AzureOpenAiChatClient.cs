using Azure;
using Azure.AI.OpenAI;
using GraphRag.AI.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace GraphRag.AI.ChatClients
{
    public class AzureOpenAiChatClient : IChatClient, IDisposable
    {
        private readonly IChatClient _chatClient;

        public AzureOpenAiChatClient(IOptions<AzureOpenAiOptions> aiOptions)
        {
            AzureOpenAiOptions options = aiOptions.Value;

            ArgumentNullException.ThrowIfNull(options.Endpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.Endpoint.ToString());
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ChatDeploymentName);

            var client = new AzureOpenAIClient(
                    options.Endpoint,
                    new AzureKeyCredential(options.ApiKey));

            _chatClient = client
                .GetChatClient(options.ChatDeploymentName)
                .AsIChatClient();
        }

        public AzureOpenAiChatClient(AzureOpenAiOptions options)
        {
            ArgumentNullException.ThrowIfNull(options.Endpoint);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.Endpoint.ToString());
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(options.ChatDeploymentName);

            var client = new AzureOpenAIClient(
                    options.Endpoint,
                    new AzureKeyCredential(options.ApiKey));

            _chatClient = client
                .GetChatClient(options.ChatDeploymentName)
                .AsIChatClient();
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
