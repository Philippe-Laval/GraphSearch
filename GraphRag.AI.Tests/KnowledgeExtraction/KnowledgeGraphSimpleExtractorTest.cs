using GraphRag.AI.ChatClients;
using GraphRag.AI.Extensions;
using GraphRag.AI.KnowledgeExtraction;
using GraphRag.Core.Configuration;
using GraphRag.Core.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using static OllamaSharp.OllamaApiClient;

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
                .Build();
        }


        [TestMethod]
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

            // Assert
            Assert.IsNotNull(result);

            Assert.HasCount(0, result.Nodes);
            Assert.HasCount(0, result.Edges);
        }
    }
}
