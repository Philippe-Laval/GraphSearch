using GraphRag.AI.Algorithms;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace GraphRag.AI.Tests;

[TestClass]
public sealed class ApplicationTests
{
    private static IChatClient CreateOllamaClient() =>
       new OllamaApiClient("http://localhost:11434", "gemma4:e4b");


    [TestMethod]
    public async Task Services_CanResolveKnowledgeGraphExtractor()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddSingleton<IChatClient>(serviceProvider => CreateOllamaClient());

        // Add logging services
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddTransient<KnowledgeGraphExtractor>();
        services.AddTransient<Application>();

        using ServiceProvider provider =
            services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

        // Act
        Application? application =
            provider.GetService<Application>();

        // Assert
        Assert.IsNotNull(application);

        // Act
        string json = await application.RunAsync();

        // Assert
        Assert.IsNotNull(json);
    }
}