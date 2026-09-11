using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OpenAI;
using GraphRag.Core.Configuration;
using System.ClientModel;

namespace GraphRag.AI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAiConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AiOptions>()
            .Bind(configuration.GetRequiredSection(AiOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AiOptions>, AiOptionsValidator>();
        return services;
    }

    public static IServiceCollection AddConfiguredChatClient(this IServiceCollection services)
    {
        services.AddSingleton<IChatClient>(serviceProvider =>
        {
            AiOptions options = serviceProvider
                .GetRequiredService<IOptions<AiOptions>>()
                .Value;

            return options.Provider.Trim() switch
            {
                AiProviders.OpenAI => CreateOpenAiClient(options.OpenAI),
                AiProviders.AzureOpenAI => CreateAzureOpenAiClient(options.AzureOpenAI),
                AiProviders.Ollama => CreateOllamaClient(options.Ollama),
                _ => throw new InvalidOperationException(
                    $"Unsupported AI provider '{options.Provider}'.")
            };
        });

        return services;
    }

    private static IChatClient CreateOpenAiClient(OpenAiOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);

        OpenAIClient client = options.Endpoint is null
            ? new OpenAIClient(options.ApiKey)
            : new OpenAIClient(
                new ApiKeyCredential(options.ApiKey),
                new OpenAIClientOptions { Endpoint = options.Endpoint });

        return client
            .GetChatClient(options.Model)
            .AsIChatClient();
    }

    private static IChatClient CreateAzureOpenAiClient(AzureOpenAiOptions options)
    {
        ArgumentNullException.ThrowIfNull(options.Endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ApiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ChatDeploymentName);

        return new AzureOpenAIClient(
                options.Endpoint,
                new AzureKeyCredential(options.ApiKey))
            .GetChatClient(options.ChatDeploymentName)
            .AsIChatClient();
    }

    private static IChatClient CreateOllamaClient(OllamaOptions options)
    {
        return new OllamaApiClient(options.Endpoint, options.Model);
    }
}
