using GraphRag.AI.Extensions;
using GraphRag.Core.KnowledgeExtraction;
using GraphRag.AI.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using GrapRag.Core.KnowledgeExtraction;

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
        public async Task Services_CanResolveKnowledgeGraphSimpleExtractor_WithFakeIChatClient()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IChatClient>(new FakeChatClientWithJson(CreateGraph()));
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug));
            services.AddTransient<IKnowledgeGraphSimpleExtractor, KnowledgeGraphSimpleExtractor>();

            using ServiceProvider provider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

            IKnowledgeGraphSimpleExtractor? extractor = provider.GetService<IKnowledgeGraphSimpleExtractor>();

            Assert.IsNotNull(extractor);

            KnowledgeGraph result = await extractor.ExtractAsync(CreateSourceText());

            Assert.HasCount(2, result.Nodes);
            Assert.HasCount(1, result.Edges);
        }

        [TestMethod]
        public async Task ExtractAsync_ReturnsStructuredKnowledgeGraph_FromFakeIChatClient()
        {
            KnowledgeGraph expectedGraph = CreateGraph();
            IChatClient chatClient = new FakeChatClientWithJson(expectedGraph);
            var extractor = CreateExtractor(chatClient);

            KnowledgeGraph result = await extractor.ExtractAsync(CreateSourceText());

            Assert.HasCount(2, result.Nodes);
            Assert.HasCount(1, result.Edges);

            KnowledgeNode aiNode = result.Nodes.Single(node => node.Id == "topic-1");
            Assert.AreEqual("Microsoft.Extensions.AI", aiNode.Name);
            Assert.AreEqual("Technology", aiNode.Type);
            Assert.AreEqual(0.95, aiNode.Importance);

            KnowledgeEdge edge = result.Edges.Single();
            Assert.AreEqual("topic-1", edge.SourceId);
            Assert.AreEqual("topic-2", edge.TargetId);
            Assert.AreEqual("IMPLEMENTS", edge.Relation);
            Assert.AreEqual(0.92, edge.Confidence);
        }

        [TestMethod]
        public async Task ExtractAsync_SendsDocumentTextAndStructuredOutputOptions()
        {
            IReadOnlyList<ChatMessage>? capturedMessages = null;
            ChatOptions? capturedOptions = null;

            IChatClient chatClient = new FakeChatClient((messages, options, _) =>
            {
                capturedMessages = messages.ToList();
                capturedOptions = options;

                return Task.FromResult(
                    new ChatResponse(
                        new ChatMessage(
                            ChatRole.Assistant,
                            KnowledgeGraphSerialization.Serialize(CreateGraph()))));
            });

            var extractor = CreateExtractor(chatClient);
            const string text = "The IChatClient interface abstracts chat-based language models.";

            await extractor.ExtractAsync(text);

            Assert.IsNotNull(capturedMessages);

            string combinedText = string.Join(
                Environment.NewLine,
                capturedMessages.Select(message => message.Text));

            StringAssert.Contains(combinedText, "Extract the main topics and their relationships");
            StringAssert.Contains(combinedText, text);
            Assert.IsNotNull(capturedOptions);
            Assert.IsNotNull(capturedOptions.ResponseFormat);
        }

        [TestMethod]
        public async Task ExtractAsync_WhenGraphContainsUnknownTarget_ThrowsInvalidOperationException()
        {
            KnowledgeGraph invalidGraph = new()
            {
                Nodes =
                [
                    new KnowledgeNode
                    {
                        Id = "topic-1",
                        Name = "IChatClient",
                        Type = "Concept",
                        Description = "Chat client abstraction.",
                        Importance = 0.8,
                        Evidence = "The IChatClient interface abstracts chat-based language models."
                    }
                ],
                Edges =
                [
                    new KnowledgeEdge
                    {
                        SourceId = "topic-1",
                        TargetId = "missing-topic",
                        Relation = "USES",
                        Description = "Invalid edge for testing.",
                        Confidence = 0.7,
                        Evidence = "The IChatClient interface abstracts chat-based language models."
                    }
                ]
            };

            IChatClient chatClient = new FakeChatClientWithJson(invalidGraph);
            var extractor = CreateExtractor(chatClient);

            InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => extractor.ExtractAsync(CreateSourceText()));

            StringAssert.Contains(exception.Message, "Unknown target node 'missing-topic'");
        }

        [TestMethod]
        public async Task ExtractAsync_WhenChatClientFails_PropagatesException()
        {
            IChatClient chatClient = new FakeChatClient(
                (_, _, _) => throw new HttpRequestException("The AI provider is unavailable."));

            var extractor = CreateExtractor(chatClient);

            await Assert.ThrowsExactlyAsync<HttpRequestException>(
                () => extractor.ExtractAsync(CreateSourceText()));
        }

        /// <summary>
        /// Crée une instance de KnowledgeGraphSimpleExtractor avec le client de chat fourni et un journaliseur
        /// configuré au niveau Debug.
        /// </summary>
        /// <param name="chatClient">Client de chat utilisé par l’extracteur pour traiter les interactions avec le modèle.</param>
        /// <returns>Nouvelle instance de KnowledgeGraphSimpleExtractor initialisée avec le client fourni et un journaliseur
        /// Debug.</returns>
        private static KnowledgeGraphSimpleExtractor CreateExtractor(IChatClient chatClient)
        {
            return new KnowledgeGraphSimpleExtractor(
                chatClient,
                LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Debug))
                    .CreateLogger<KnowledgeGraphSimpleExtractor>());
        }

        private static string CreateSourceText() =>
            """
            Microsoft.Extensions.AI provides abstractions for integrating
            artificial intelligence services into .NET applications.

            The IChatClient interface abstracts chat-based language models.
            Applications can use implementations backed by OpenAI,
            Azure OpenAI or local models.

            Structured output can be used to deserialize model responses
            into strongly typed C# objects.
            """;

        private static KnowledgeGraph CreateGraph() =>
            new()
            {
                Nodes =
                [
                    new KnowledgeNode
                    {
                        Id = "topic-1",
                        Name = "Microsoft.Extensions.AI",
                        Type = "Technology",
                        Description = "Provides abstractions for integrating AI services into .NET applications.",
                        Importance = 0.95,
                        Evidence = "Microsoft.Extensions.AI provides abstractions for integrating artificial intelligence services into .NET applications."
                    },
                    new KnowledgeNode
                    {
                        Id = "topic-2",
                        Name = "IChatClient",
                        Type = "Concept",
                        Description = "An interface that abstracts chat-based language models.",
                        Importance = 0.90,
                        Evidence = "The IChatClient interface abstracts chat-based language models."
                    }
                ],
                Edges =
                [
                    new KnowledgeEdge
                    {
                        SourceId = "topic-1",
                        TargetId = "topic-2",
                        Relation = "IMPLEMENTS",
                        Description = "Microsoft.Extensions.AI exposes abstractions centered around IChatClient.",
                        Confidence = 0.92,
                        Evidence = "The IChatClient interface abstracts chat-based language models."
                    }
                ]
            };



        [TestMethod]
        [CICondition(ConditionMode.Exclude)]
        [TestCategory("AI")]
        [Ignore("Call a real IChatClient, this has a cost")]
        public async Task ExtractAsync_InstantiateWithDIAndRealIChatClient()
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
            string text = CreateSourceText();

            KnowledgeGraph result = await extractor.ExtractAsync(text);

            string json = KnowledgeGraphSerialization.Serialize(result);
            Console.WriteLine(json);

            #region example of output json
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
            #endregion

            // Assert
            Assert.IsNotNull(result);

            Assert.IsNotEmpty(result.Nodes);
            Assert.IsNotEmpty(result.Edges);
        }

    }
}