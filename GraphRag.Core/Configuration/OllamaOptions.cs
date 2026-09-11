using System.ComponentModel.DataAnnotations;

namespace GraphRag.Core.Configuration;

// Embedding model reference: https://ollama.com/search?c=embedding

public sealed class OllamaOptions
{
    [Required]
    public Uri Endpoint { get; init; } = new("http://localhost:11434");

    [Required]
    public string ChatModel { get; init; } = "gemma4:e4b";
    
    [Required]
    public string EmbeddingModel { get; init; } = "embeddinggemma";
}
