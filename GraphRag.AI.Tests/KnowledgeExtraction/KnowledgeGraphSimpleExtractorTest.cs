using GraphRag.AI.ChatClients;
using GraphRag.AI.Extensions;
using GraphRag.AI.KnowledgeExtraction;
using GraphRag.Core.Configuration;
using GraphRag.Core.KnowledgeExtraction;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using static OllamaSharp.OllamaApiClient;

// Test execution and control in MSTest
// https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-writing-tests-controlling-execution

// Test organization and metadata in MSTest
// https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-writing-tests-organizing

namespace GraphRag.AI.Tests.KnowledgeExtraction
{
    [TestClass]
    public sealed class KnowledgeGraphSimpleExtractorTest
    {
        private readonly IConfiguration _configuration;

        public KnowledgeGraphSimpleExtractorTest()
        {       
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(
                    path: "appsettings.json",
                    optional: false,
                    reloadOnChange: false)
                // The API keys for OpenAI, Azure OpenAI or Ollama should be set in environment variables or in appsettings.json.
                .AddEnvironmentVariables()
                .Build();
        }


        [TestMethod]
        [CICondition(ConditionMode.Exclude)]
        [TestCategory("AI")]
        [Ignore("Call AI this has a cost")]
        public async Task ExtractAsync_Test()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddAiConfiguration(_configuration);
            services.AddConfiguredChatClient();
            services.AddTransient<IKnowledgeGraphSimpleExtractor, KnowledgeGraphSimpleExtractor>();

            // Add logging services
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });


            using ServiceProvider provider =
                services.BuildServiceProvider(
                    new ServiceProviderOptions
                    {
                        ValidateOnBuild = true,
                        ValidateScopes = true
                    });

            // Act
            IKnowledgeGraphSimpleExtractor? extractor =
                provider.GetService<IKnowledgeGraphSimpleExtractor>();

            // Assert
            Assert.IsNotNull(extractor);

            // Act
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

            KnowledgeGraph result = await extractor.ExtractAsync(
                text);

            string json = KnowledgeGraphSerialization.Serialize(result);
            Console.WriteLine(json);

            /*
{
  "nodes": [
    {
      "id": "topic-1",
      "name": "Microsoft.Extensions.AI",
      "type": "Organization",
      "description": "Provides abstractions for integrating AI services into .NET applications.",
      "aliases": [],
      "importance": 1,
      "evidence": "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
    },
    {
      "id": "topic-2",
      "name": "IChatClient",
      "type": "Concept",
      "description": "An interface that abstracts chat-based language models.",
      "aliases": [],
      "importance": 1,
      "evidence": "The IChatClient interface abstracts chat-based language models."
    },
    {
      "id": "topic-3",
      "name": "OpenAI",
      "type": "Organization",
      "description": "A provider of AI services that can back implementations used with IChatClient.",
      "aliases": [],
      "importance": 0.8,
      "evidence": "Applications can use implementations backed by OpenAI."
    },
    {
      "id": "topic-4",
      "name": "Azure OpenAI",
      "type": "Organization",
      "description": "A provider of AI services that can back implementations used with IChatClient.",
      "aliases": [],
      "importance": 0.8,
      "evidence": "Applications can use implementations backed by Azure OpenAI."
    },
    {
      "id": "topic-5",
      "name": "Local Models",
      "type": "Concept",
      "description": "Refers to AI models that are run locally, as opposed to using cloud-based services.",
      "aliases": [],
      "importance": 0.6,
      "evidence": "Applications can use implementations backed by local models."
    },
    {
      "id": "topic-6",
      "name": "Structured Output",
      "type": "Concept",
      "description": "Can be used to deserialize model responses into strongly typed C# objects.",
      "aliases": [],
      "importance": 0.7,
      "evidence": "Structured output can be used to deserialize model responses into strongly typed C# objects."
    }
  ],
  "edges": [
    {
      "sourceId": "topic-1",
      "targetId": "topic-2",
      "relation": "IMPLEMENTS",
      "description": "Microsoft.Extensions.AI implements the IChatClient interface for language models.",
      "confidence": 0.9,
      "evidence": "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
    },
    {
      "sourceId": "topic-2",
      "targetId": "topic-3",
      "relation": "USES",
      "description": "IChatClient can use implementations backed by OpenAI.",
      "confidence": 0.8,
      "evidence": "Applications can use implementations backed by OpenAI."
    },
    {
      "sourceId": "topic-2",
      "targetId": "topic-4",
      "relation": "USES",
      "description": "IChatClient can use implementations backed by Azure OpenAI.",
      "confidence": 0.8,
      "evidence": "Applications can use implementations backed by Azure OpenAI."
    },
    {
      "sourceId": "topic-2",
      "targetId": "topic-5",
      "relation": "USES",
      "description": "IChatClient can use implementations backed by local models.",
      "confidence": 0.7,
      "evidence": "Applications can use implementations backed by local models."
    },
    {
      "sourceId": "topic-1",
      "targetId": "topic-6",
      "relation": "PRODUCES",
      "description": "Microsoft.Extensions.AI produces structured output for model response deserialization.",
      "confidence": 0.75,
      "evidence": "Structured output can be used to deserialize model responses into strongly typed C# objects."
    }
  ]
}             
             */


            // Assert
            Assert.IsNotNull(result);

            Assert.IsNotEmpty(result.Nodes);
            Assert.IsNotEmpty(result.Edges);
        }
    }
}
