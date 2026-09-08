namespace GraphSearch.Library.Query.Analysis.Llm;

/// <summary>
/// Minimal abstraction over a chat/completion LLM. Adapt to your provider
/// (OpenAI / Azure OpenAI / Ollama / local) in the composition root.
/// </summary>
public interface IChatCompletionClient
{
    /// <summary>
    /// Sends the given prompt and returns the raw completion text.
    /// </summary>
    Task<string> CompleteAsync(
        string prompt,
        CancellationToken cancellationToken = default);
}
