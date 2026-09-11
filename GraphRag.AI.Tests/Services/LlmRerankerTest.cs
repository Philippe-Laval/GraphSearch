using GraphRag.AI.Algorithms;
using GraphRag.Core.Configuration;
using GraphRag.AI.Extensions;
using GraphRag.AI.Services;
using GraphRag.Core.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace GraphRag.AI.Tests
{
    [TestClass]
    public sealed class LlmRerankerTest
    {
        private readonly IConfiguration _configuration;

        public LlmRerankerTest()
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
        public async Task TestReRankAsync()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddAiConfiguration(_configuration);
            services.AddConfiguredChatClient();

            services.AddSingleton<ILlmReranker, LlmReranker>();

            // Add logging services
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            //services.AddTransient<KnowledgeGraphExtractor>();

            using ServiceProvider provider =
                services.BuildServiceProvider(
                    new ServiceProviderOptions
                    {
                        ValidateOnBuild = true,
                        ValidateScopes = true
                    });

            // Act

            AiOptions options = provider
                .GetRequiredService<IOptions<AiOptions>>()
                .Value;

            ILlmReranker? reranker =
                provider.GetService<ILlmReranker>();

            // Assert
            Assert.IsNotNull(reranker);

            string query = "What is a LLM";
            string document = "An LLM is a Large Language Model: an AI system trained on very large amounts of text so it can understand and generate human-like language.";

            var result = await reranker!.ReRankAsync(query, document, CancellationToken.None);

            /* In Ollama UI, paste the prompt below to get the expected output:
             
You are an expert at evaluating document relevance.
Your task is to determine how relevant a document is for answering a specific query.

Query: What is a LLM
Document: An LLM is a Large Language Model: an AI system trained on very large amounts of text so it can understand and generate human-like language.

Analyze the document and determine its relevance to the query.
Consider:
1. How directly the document answers the query
2. The quality and specificity of the information provided
3. The semantic relationship between the query and document content

Provide your response as a JSON object with the following structure:
{
"relevance_score": <number between 0.0 and 1.0>,
"explanation": "<brief explanation of the relevance score>"
}

The relevance_score should be:
- 0.0-0.2: Not relevant or completely off-topic
- 0.2-0.4: Somewhat relevant but lacks specificity
- 0.4-0.6: Moderately relevant with some useful information
- 0.6-0.8: Highly relevant with good information
- 0.8-1.0: Extremely relevant and directly answers the query

Response : 
{
  "relevance_score": 0.95,
  "explanation": "The document provides a direct definition of an LLM (Large Language Model), explicitly stating it's an AI system trained on text to understand and generate language. This aligns perfectly with the query 'What is a LLM'. The information is clear, concise, and specific, making it highly relevant."
}
             */


        }
    }
}
