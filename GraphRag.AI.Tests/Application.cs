using GraphRag.AI.Algorithms;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphRag.AI.Tests;

public sealed class Application(
    KnowledgeGraphExtractor extractor)
{
    public async Task<string> RunAsync(
        CancellationToken cancellationToken = default)
    {
        const string text =
            """
            Microsoft.Extensions.AI provides abstractions for integrating
            artificial intelligence services into .NET applications.

            The IChatClient interface abstracts chat-based language models.
            Applications can use implementations backed by OpenAI,
            Azure OpenAI or local models.

            Structured output can be used to deserialize model responses
            into strongly typed C# objects.
            """;

        KnowledgeGraph graph =
            await extractor.ExtractChunckAsync(text, cancellationToken);

        string json = JsonSerializer.Serialize(
            graph,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        Console.WriteLine(json);

        return json;
    }
}


/*
"""
            Microsoft.Extensions.AI provides abstractions for integrating
            artificial intelligence services into .NET applications.

            The IChatClient interface abstracts chat-based language models.
            Applications can use implementations backed by OpenAI,
            Azure OpenAI or local models.

            Structured output can be used to deserialize model responses
            into strongly typed C# objects.
            """; 
 */

/* 

{
  "Nodes": [
    {
      "Id": "Microsoft.Extensions.AI",
      "Name": "Microsoft.Extensions.AI",
      "Type": "Module/Library",
      "Description": "provides abstractions for integrating artificial intelligence services into .NET applications.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
    },
    {
      "Id": "IChatClient",
      "Name": "IChatClient",
      "Type": "Interface",
      "Description": "abstracts chat-based language models.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "The IChatClient interface abstracts chat-based language models."
    },
    {
      "Id": "AI_SERVICES",
      "Name": "artificial intelligence services",
      "Type": "Service/Concept",
      "Description": "services that can be integrated into .NET applications.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
    },
    {
      "Id": "OpenAI",
      "Name": "OpenAI",
      "Type": "Provider/Model Source",
      "Description": "A possible implementation provider for chat-based language models.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "Applications can use implementations backed by OpenAI, Azure OpenAI or local models."
    },
    {
      "Id": "Azure OpenAI",
      "Name": "Azure OpenAI",
      "Type": "Provider/Model Source",
      "Description": "A possible implementation provider for chat-based language models.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "Applications can use implementations backed by OpenAI, Azure OpenAI or local models."
    },
    {
      "Id": "local models",
      "Name": "local models",
      "Type": "Model Type",
      "Description": "Another type of model available for implementation.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "Applications can use implementations backed by OpenAI, Azure OpenAI or local models."
    },
    {
      "Id": ".NET applications",
      "Name": ".NET applications",
      "Type": "Platform/Target",
      "Description": "The target environment for integrating AI services.",
      "Aliases": [],
      "Importance": 0,
      "Evidence": "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
    }
  ],
  "Edges": [
    {
      "SourceId": "Microsoft.Extensions.AI",
      "TargetId": "AI_SERVICES",
      "Relation": "INTEGRATES_SERVICE",
      "Description": "provides abstractions for integrating AI SERVICES.",
      "Confidence": 0,
      "Evidence": "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
    },
    {
      "SourceId": "OpenAI",
      "TargetId": "IChatClient",
      "Relation": "IMPLEMENTS",
      "Description": "is a possible backend implementation for chat clients.",
      "Confidence": 0,
      "Evidence": "Applications can use implementations backed by OpenAI, Azure OpenAI or local models."
    },
    {
      "SourceId": "Azure OpenAI",
      "TargetId": "IChatClient",
      "Relation": "IMPLEMENTS",
      "Description": "is a possible backend implementation for chat clients.",
      "Confidence": 0,
      "Evidence": "Applications can use implementations backed by OpenAI, Azure OpenAI or local models."
    },
    {
      "SourceId": "local models",
      "TargetId": "IChatClient",
      "Relation": "IMPLEMENTS",
      "Description": "is a possible backend implementation for chat clients.",
      "Confidence": 0,
      "Evidence": "Applications can use implementations backed by OpenAI, Azure OpenAI or local models."
    }
  ]
}
 */