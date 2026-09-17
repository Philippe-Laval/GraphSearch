using System.ComponentModel.DataAnnotations;

namespace GraphRag.AI.Configuration;

public sealed class AzureOpenAiOptions
{
    /// <summary>
    /// Endpoint for the service.
    /// </summary>
    [Required]
    public Uri? Endpoint { get; init; }

    /// <summary>
    /// API key
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Model used for chat
    /// </summary>
    [Required] 
    public string? ChatDeploymentName { get; init; } = "gpt-4o-mini";

    /// <summary>
    /// Model used for creating embedding
    /// </summary>
    [Required]
    public string EmbeddingDeploymentName { get; init; } = "text-embedding-ada-002";
}