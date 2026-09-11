using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.AI;

namespace GraphRag.AI.Tests;

/// <summary>
/// A handwritten fake is often preferable to a mocking framework because 
/// LLM responses can contain structured messages, metadata, tool calls, and streaming responses.
/// </summary>
public sealed class FakeChatClient : IChatClient
{
    private readonly Func<
        IEnumerable<ChatMessage>,
        ChatOptions?,
        CancellationToken,
        Task<ChatResponse>> _responseFactory;

    public FakeChatClient(string responseText)
        : this((_, _, _) => Task.FromResult(
            new ChatResponse(
                new ChatMessage(ChatRole.Assistant, responseText))))
    {
    }

    public FakeChatClient(
        Func<
            IEnumerable<ChatMessage>,
            ChatOptions?,
            CancellationToken,
            Task<ChatResponse>> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public ChatClientMetadata Metadata { get; } =
        new("FakeChatClient", providerUri: null, defaultModelId: "fake-model");

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return _responseFactory(messages, options, cancellationToken);
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        ChatResponse response =
            await GetResponseAsync(messages, options, cancellationToken);

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
        return serviceType.IsInstanceOfType(this)
            ? this
            : null;
    }

    public void Dispose()
    {
    }
}
