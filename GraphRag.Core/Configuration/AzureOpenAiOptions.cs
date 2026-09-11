using System.ComponentModel.DataAnnotations;

namespace GraphRag.Core.Configuration;

public sealed class AzureOpenAiOptions
{
    public Uri? Endpoint { get; init; }

    public string? ApiKey { get; init; }

    public string? ChatDeploymentName { get; init; } = "gpt-4o-mini";

    public string EmbeddingDeploymentName { get; init; } = "text-embedding-ada-002";
}