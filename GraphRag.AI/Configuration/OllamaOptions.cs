using System.ComponentModel.DataAnnotations;

namespace GraphRag.AI.Configuration;

// Embedding model reference: https://ollama.com/search?c=embedding

public sealed class OllamaOptions
{
    /// <summary>
    /// Endpoint for the service.
    /// </summary>
    [Required]
    public Uri Endpoint { get; init; } = new("http://localhost:11434");

    /// <summary>
    /// Model used for chat
    /// </summary>
    [Required]
    public string ChatModel { get; init; } = "gemma4:e4b";

    /// <summary>
    /// Model used for creating embedding
    /// </summary>
    [Required]
    public string EmbeddingModel { get; init; } = "embeddinggemma";
}
