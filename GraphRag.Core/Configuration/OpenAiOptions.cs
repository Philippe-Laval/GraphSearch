using System.ComponentModel.DataAnnotations;

namespace GraphRag.Core.Configuration;

public sealed class OpenAiOptions
{
    /// <summary>
    /// Optional OpenAI-compatible endpoint.
    /// </summary>
    public Uri? Endpoint { get; init; }

    /// <summary>
    /// API key
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Model used for chat
    /// </summary>
    [Required]
    public string ChatModel { get; init; } = "gpt-4.1-mini";

    /// <summary>
    /// Model used for creating embedding
    /// </summary>
    [Required]
    public string EmbeddingModel { get; init; } = "text-embedding-3-small";
}
