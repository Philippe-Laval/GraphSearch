using GraphRag.Core.Interfaces;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.AI.Algorithms;

/// <summary>
/// MafKnowledgeAgent is an implementation of IKnowledgeTextGenerationAgent that uses an IChatClient to generate text based on a given prompt. It provides an asynchronous method to generate text responses, making it suitable for scenarios where knowledge extraction and text generation are required.
/// MAF : Microsoft Agent Framework
/// </summary>
public sealed class MafKnowledgeAgent : IKnowledgeTextGenerationAgent
{
    private readonly IChatClient _chat;

    public MafKnowledgeAgent(IChatClient chat)
    {
        _chat = chat;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        var response = await _chat.GetResponseAsync(prompt, cancellationToken: cancellationToken);
        return response.Messages[0].Text;
    }
}
