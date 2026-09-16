using GraphRag.Core.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.AI.ChatClients
{
    public class OpenAiChatClient : IChatClient, IDisposable
    {
        private readonly IChatClient _chatClient;

        public OpenAiChatClient(IOptions<OpenAiOptions> aiOptions)
        {
            OpenAiOptions options = aiOptions.Value;

            ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);

            OpenAIClient client = options.Endpoint is null
                ? new OpenAIClient(options.ApiKey)
                : new OpenAIClient(
                    new ApiKeyCredential(options.ApiKey),
                    new OpenAIClientOptions { Endpoint = options.Endpoint });

            _chatClient = client
                .GetChatClient(options.ChatModel)
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
