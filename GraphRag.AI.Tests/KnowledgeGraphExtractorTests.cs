using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using GraphRag.AI.Algorithms;
using GraphRag.Core.Models;

namespace GraphRag.AI.Tests
{
    [TestClass]
    public sealed class KnowledgeGraphExtractorTests
    {
        [TestMethod]
        public async Task Services_CanResolveKnowledgeGraphExtractor()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddSingleton<IChatClient>(
                new FakeChatClientWithJson("""{"nodes": [], "edges": []}""")
                );

            // Add logging services
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddTransient<KnowledgeGraphExtractor>();

            using ServiceProvider provider =
                services.BuildServiceProvider(
                    new ServiceProviderOptions
                    {
                        ValidateOnBuild = true,
                        ValidateScopes = true
                    });

            // Act
            KnowledgeGraphExtractor? extractor =
                provider.GetService<KnowledgeGraphExtractor>();

            // Assert
            Assert.IsNotNull(extractor);

            // Act
            KnowledgeGraph result = await extractor.ExtractChunckAsync(
                "Alice works for Contoso.");

            // Assert
            Assert.IsNotNull(result);

            Assert.HasCount(0, result.Nodes);
            Assert.HasCount(0, result.Edges);
        }

        [TestMethod]
        public async Task ExtractAsync_ReturnsStructuredKnowledgeGraph()
        {
            // Arrange
            var expectedGraph = new KnowledgeGraph
            {
                Nodes =
                [
                    new KnowledgeNode
                {
                    Id = "person-1",
                    Name = "Alice",
                    Type = "Person",
                    Description = "An employee of Contoso.",
                    Aliases = [],
                    Importance = 0.8,
                    Evidence = "Alice works for Contoso."
                },
                new KnowledgeNode
                {
                    Id = "organization-1",
                    Name = "Contoso",
                    Type = "Organization",
                    Description = "The organization employing Alice.",
                    Aliases = [],
                    Importance = 0.9,
                    Evidence = "Alice works for Contoso."
                }
                ],
                Edges =
                [
                    new KnowledgeEdge
                {
                    SourceId = "person-1",
                    TargetId = "organization-1",
                    Relation = "WORKS_FOR",
                    Description = "Alice works for Contoso.",
                    Confidence = 0.98,
                    Evidence = "Alice works for Contoso."
                }
                ]
            };

            IChatClient chatClient = new FakeChatClientWithJson(expectedGraph);


            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<KnowledgeGraphExtractor>();

            // For a unit test, direct construction is clearer than using dependency injection.
            var sut = new KnowledgeGraphExtractor(chatClient, logger);

            // Act
            KnowledgeGraph result = await sut.ExtractChunckAsync(
                "Alice works for Contoso.");

            // Assert
            Assert.IsNotNull(result);

            Assert.HasCount(2, result.Nodes);
            Assert.HasCount(1, result.Edges);

            KnowledgeNode alice = result.Nodes.Single(
                node => node.Id == "person-1");

            Assert.AreEqual("Alice", alice.Name);
            Assert.AreEqual("Person", alice.Type);
            Assert.AreEqual(0.8, alice.Importance);

            KnowledgeEdge edge = result.Edges.Single();

            Assert.AreEqual("person-1", edge.SourceId);
            Assert.AreEqual("organization-1", edge.TargetId);
            Assert.AreEqual("WORKS_FOR", edge.Relation);
            Assert.AreEqual(0.98, edge.Confidence);
        }

        /// <summary>
        /// You can also verify that the fake works directly with GetResponseAsync<KnowledgeGraph>()
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetResponseAsync_DeserializesStructuredResponse()
        {
            // Arrange
            var expectedGraph = new KnowledgeGraph
            {
                Nodes =
                [
                    new KnowledgeNode
                    {
                        Id = "technology-1",
                        Name = ".NET",
                        Type = "Technology",
                        Description = "A software development platform.",
                        Aliases = ["dotnet"],
                        Importance = 0.95,
                        Evidence = ".NET is used to build the application."
                    }
                ],
                Edges = []
            };

            IChatClient chatClient = new FakeChatClientWithJson(expectedGraph);

            ChatMessage[] messages =
            [
                new(
                    ChatRole.User,
                    ".NET is used to build the application.")
            ];

            // Act
            ChatResponse<KnowledgeGraph> response =
                await chatClient.GetResponseAsync<KnowledgeGraph>(messages);

            // Assert
            Assert.IsNotNull(response.Result);
            Assert.HasCount(1, response.Result.Nodes);
            Assert.AreEqual(".NET", response.Result.Nodes[0].Name);
            Assert.IsEmpty(response.Result.Edges);
        }

        /// <summary>
        /// For structured output, it can be useful to verify both the prompt 
        /// and the options passed to the underlying client
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ExtractAsync_SendsInputAndStructuredOutputOptions()
        {
            // Arrange
            IReadOnlyList<ChatMessage>? capturedMessages = null;
            ChatOptions? capturedOptions = null;

            var graph = new KnowledgeGraph
            {
                Nodes = [],
                Edges = []
            };

            var chatClient = new FakeChatClientWithJson(
                (messages, options, _) =>
                {
                    // The fake can also capture messages
                    capturedMessages = messages.ToList();
                    capturedOptions = options;

                    string json = JsonSerializer.Serialize(
                        graph,
                        FakeChatClientWithJson.SerializerOptions);

                    return Task.FromResult(
                        new ChatResponse(
                            new ChatMessage(ChatRole.Assistant, json)));
                });

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<KnowledgeGraphExtractor>();

            var sut = new KnowledgeGraphExtractor(chatClient, logger);

            // Act
            await sut.ExtractChunckAsync(
                "Alice works for Contoso.");

            // Assert
            Assert.IsNotNull(capturedMessages);

            string combinedText = string.Join(
                Environment.NewLine,
                capturedMessages.Select(message => message.Text));

            StringAssert.Contains(
                combinedText,
                "Alice works for Contoso.");

            // The generic extension prepares the structured response format before calling the underlying IChatClient,
            // so the fake can inspect ChatOptions.ResponseFormat.
            // Avoid testing every internal schema detail, because that would couple your tests
            // to the implementation of Microsoft.Extensions.AI.
            Assert.IsNotNull(capturedOptions);
            Assert.IsNotNull(capturedOptions.ResponseFormat);
        }

        /// <summary>
        /// The typed API should also be tested when the model returns invalid JSON
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ExtractAsync_WhenResponseIsInvalidJson_ThrowsException()
        {
            // Arrange
            IChatClient chatClient = new FakeChatClientWithJson(
                "This is not valid JSON.");

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<KnowledgeGraphExtractor>();

            var sut = new KnowledgeGraphExtractor(chatClient, logger);

            // Act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await sut.ExtractChunckAsync("Alice works for Contoso.");
            });
        }

        [TestMethod]
        public async Task ExtractAsync_WhenResponseIsInvalid_ThrowsException()
        {
            IChatClient chatClient = new FakeChatClientWithJson(
                """{"nodes": "not-an-array", "edges": []}""");

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<KnowledgeGraphExtractor>();

            var sut = new KnowledgeGraphExtractor(chatClient, logger);

            // Depending on the package version, the deserialization failure may be wrapped in another exception.
            // A less version-sensitive test is
            await Assert.ThrowsAsync<Exception>(async () => {
                await sut.ExtractChunckAsync("Alice works for Contoso.");
            });
        }

        /// <summary>
        /// You should also test failures independently of a real provider
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        [TestMethod]
        public async Task ExtractAsync_WhenChatClientFails_PropagatesException()
        {
            // Arrange
            var chatClient = new FakeChatClientWithJson(
                (_, _, _) => throw new HttpRequestException(
                    "The AI provider is unavailable."));

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<KnowledgeGraphExtractor>();

            var sut = new KnowledgeGraphExtractor(chatClient, logger);

            // Act and assert
            await Assert.ThrowsExactlyAsync<HttpRequestException>(
                () => sut.ExtractChunckAsync("Alice works for Contoso."));
        }

    }
}
