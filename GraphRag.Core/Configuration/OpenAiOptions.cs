using System.ComponentModel.DataAnnotations;

namespace GraphRag.Core.Configuration;

public sealed class OpenAiOptions
{
    // Optional OpenAI-compatible endpoint.
    public Uri? Endpoint { get; init; }

    public string? ApiKey { get; init; }

    [Required]
    public string Model { get; init; } = "gpt-4.1-mini";

    [Required]
    public string EmbeddingModel { get; init; } = "text-embedding-3-small";
}
