using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.AI;

namespace GraphRag.AI.Tests;

public sealed class FakeChatClientWithJson : IChatClient
{
    private readonly Func<
        IEnumerable<ChatMessage>,
        ChatOptions?,
        CancellationToken,
        Task<ChatResponse>> _responseFactory;

    public FakeChatClientWithJson(object structuredResponse)
        : this(
            (_, _, _) =>
            {
                string json = JsonSerializer.Serialize(
                    structuredResponse,
                    structuredResponse.GetType(),
                    SerializerOptions);

                return Task.FromResult(
                    new ChatResponse(
                        new ChatMessage(ChatRole.Assistant, json)));
            })
    {
    }

    public FakeChatClientWithJson(string rawResponse)
        : this(
            (_, _, _) => Task.FromResult(
                new ChatResponse(
                    new ChatMessage(ChatRole.Assistant, rawResponse))))
    {
    }

    public FakeChatClientWithJson(
        Func<
            IEnumerable<ChatMessage>,
            ChatOptions?,
            CancellationToken,
            Task<ChatResponse>> responseFactory)
    {
        ArgumentNullException.ThrowIfNull(responseFactory);

        _responseFactory = responseFactory;
    }

    public static JsonSerializerOptions SerializerOptions { get; } =
        new(JsonSerializerDefaults.Web);

    public ChatClientMetadata Metadata { get; } =
        new(providerName: "FakeChatClientWithJson", providerUri: null, defaultModelId: "fake-model");

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return _responseFactory(messages, options, cancellationToken);
    }

    public async IAsyncEnumerable<ChatResponseUpdate>
        GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
    {
        ChatResponse response = await GetResponseAsync(
            messages,
            options,
            cancellationToken);

        foreach (ChatMessage message in response.Messages)
        {
            yield return new ChatResponseUpdate
            {
                Role = message.Role,
                Contents = message.Contents
            };
        }
    }

    public object? GetService(
        Type serviceType,
        object? serviceKey = null)
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
