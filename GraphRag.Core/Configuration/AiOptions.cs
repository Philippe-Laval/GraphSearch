using System.ComponentModel.DataAnnotations;

namespace GraphRag.Core.Configuration;

public sealed class AiOptions
{
    public const string SectionName = "AI";

    [Required]
    public string Provider { get; init; } = AiProviders.Ollama;

    [Required]
    public OpenAiOptions OpenAI { get; init; } = new();

    [Required]
    public AzureOpenAiOptions AzureOpenAI { get; init; } = new();

    [Required]
    public OllamaOptions Ollama { get; init; } = new();

    [Range(1, 128_000)]
    public int MaxOutputTokens { get; init; } = 1_000;

    [Range(0.0, 2.0)]
    public float Temperature { get; init; } = 0.2f;

    [Required]
    public string SystemPrompt { get; init; } = "You are a helpful assistant.";
}