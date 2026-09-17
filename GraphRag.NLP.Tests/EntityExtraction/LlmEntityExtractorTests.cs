using GraphRag.AI.Extensions;
using GraphRag.NLP.EntityExtraction;
using GraphRag.NLP.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace GraphRag.NLP.Tests.EntityExtraction
{
    [TestClass]
    public sealed class LlmEntityExtractorTests
    {
        private readonly IConfiguration _configuration;

        public LlmEntityExtractorTests()
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
        public void Constructor_WithNullChatClient_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new LlmEntityExtractor(null!));
        }

        [TestMethod]
        public async Task ExtractAsync_WithBlankQuery_ThrowsArgumentException()
        {
            var extractor = CreateExtractor("[]");

            await Assert.ThrowsExactlyAsync<ArgumentException>(() => extractor.ExtractAsync("   "));
        }

        [TestMethod]
        public async Task ExtractAsync_ParsesWrappedJsonAndRelocatesEntitiesCaseInsensitively()
        {
            const string query = "microsoft builds C# tools in Seattle.";
            const string response = "Here you go:\n```json\n[{\"text\":\"MICROSOFT\",\"type\":\"Organization\",\"confidence\":0.91},{\"text\":\"seattle\",\"type\":\"Location\",\"confidence\":0.84}]\n```";

            var extractor = CreateExtractor(response);

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(query);

            Assert.HasCount(2, entities);
            AssertEntity(entities, "microsoft", "Organization", query.IndexOf("microsoft", StringComparison.Ordinal), 0.91);
            AssertEntity(entities, "Seattle", "Location", query.IndexOf("Seattle", StringComparison.Ordinal), 0.84);
        }

        [TestMethod]
        public async Task ExtractAsync_AppliesTypeAllowListAndClampsConfidence()
        {
            const string query = "Microsoft builds C# tools in Seattle.";
            const string response = """
                [
                  { "text": "Microsoft", "type": "Organization", "confidence": 1.4 },
                  { "text": "C#", "type": "ProgrammingLanguage", "confidence": 0.25 },
                  { "text": "Seattle", "type": "Location", "confidence": -0.5 }
                ]
                """;

            var extractor = CreateExtractor(
                response,
                typeAllowList: ["Organization", "Location"]);

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(query);

            Assert.HasCount(2, entities);
            AssertEntity(entities, "Microsoft", "Organization", query.IndexOf("Microsoft", StringComparison.Ordinal), 1.0);
            AssertEntity(entities, "Seattle", "Location", query.IndexOf("Seattle", StringComparison.Ordinal), 0.0);
            Assert.IsFalse(entities.Any(entity => entity.Text == "C#"));
        }

        [TestMethod]
        public async Task ExtractAsync_DiscardsEntitiesMissingFromQuery()
        {
            const string query = "Microsoft builds C# tools.";
            const string response = """
                [
                  { "text": "Contoso", "type": "Organization", "confidence": 0.8 }
                ]
                """;

            var extractor = CreateExtractor(response);

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(query);

            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task ExtractAsync_WhenResponseIsInvalidJson_ReturnsEmpty()
        {
            var extractor = CreateExtractor("This is not valid JSON.");

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync("Microsoft builds C# tools.");

            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task ExtractAsync_PrefersHigherConfidenceForOverlappingEntities()
        {
            const string query = "Microsoft Research released a paper.";
            const string response = """
                [
                  { "text": "Microsoft", "type": "Organization", "confidence": 0.40 },
                  { "text": "Microsoft Research", "type": "Organization", "confidence": 0.95 }
                ]
                """;

            var extractor = CreateExtractor(response);

            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(query);

            Assert.HasCount(1, entities);
            AssertEntity(entities, "Microsoft Research", "Organization", query.IndexOf("Microsoft Research", StringComparison.Ordinal), 0.95);
        }

        [TestMethod]
        [CICondition(ConditionMode.Exclude)]
        [TestCategory("AI")]
        [Ignore("Call real IChatCLient, this has a cost")]
        public async Task ExtractAsync_InstantiateWithDIAndRealIChatClient_ThisCreatesAnEmptyTypeAllowList()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddAiConfiguration(_configuration);
            services.AddConfiguredChatClient();
            services.AddTransient<IEntityExtractor, LlmEntityExtractor>();

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
            IEntityExtractor? extractor =
                provider.GetService<IEntityExtractor>();

            // Assert
            Assert.IsNotNull(extractor);

            string text = "John Doe is a software engineer at Microsoft working on C#. He lives in Seattle, Washington and enjoys hiking in the mountains.";
            IReadOnlyList<ExtractedEntity> entities = await extractor.ExtractAsync(text);

            // Assert
            Assert.IsNotNull(entities);
            Assert.IsNotEmpty(entities);
        }

        /// <summary>
        /// Crée une instance de <c>LlmEntityExtractor</c> configurée avec une réponse de chat simulée et une liste
        /// optionnelle de types autorisés.
        /// </summary>
        /// <param name="response">Réponse utilisée par le client de chat simulé.</param>
        /// <param name="typeAllowList">Types d’entités à autoriser lors de l’extraction, 
        /// ou <see langword="null"/> pour utiliser le comportement par défaut.</param>
        /// <returns>Instance de <c>LlmEntityExtractor</c> configurée avec les valeurs fournies.</returns>
        private static LlmEntityExtractor CreateExtractor(
            string response,
            IEnumerable<string>? typeAllowList = null)
        {
            return new LlmEntityExtractor(
                new FakeChatClient(response),
                typeAllowList: typeAllowList);
        }

        private static void AssertEntity(
            IReadOnlyList<ExtractedEntity> entities,
            string expectedText,
            string expectedType,
            int expectedStart,
            double expectedConfidence)
        {
            ExtractedEntity? entity = entities.FirstOrDefault(candidate => candidate.Text == expectedText);
            Assert.IsNotNull(entity);
            Assert.AreEqual(expectedType, entity.Type);
            Assert.AreEqual(expectedStart, entity.Start);
            Assert.AreEqual(expectedText.Length, entity.Length);
            Assert.AreEqual(expectedConfidence, entity.Confidence, 0.001);
        }
    }
}
