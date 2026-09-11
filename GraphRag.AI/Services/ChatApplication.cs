using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GraphRag.Core.Configuration;
using System.Text;

namespace GraphRag.AI.Services;

public sealed class ChatApplication(
    IChatClient chatClient,
    IOptions<AiOptions> aiOptions,
    ILogger<ChatApplication> logger)
{
    private readonly AiOptions _options = aiOptions.Value;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting chat application with provider {Provider}",
            _options.Provider);

        List<ChatMessage> history =
        [
            new(ChatRole.System, _options.SystemPrompt)
        ];

        Console.WriteLine($"Provider: {_options.Provider}");
        Console.WriteLine("Enter /exit to stop, or /clear to clear the conversation.");

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("\nYou: ");
            string? prompt = Console.ReadLine();

            if (prompt is null || prompt.Equals("/exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (prompt.Equals("/clear", StringComparison.OrdinalIgnoreCase))
            {
                history = [new ChatMessage(ChatRole.System, _options.SystemPrompt)];
                Console.WriteLine("Conversation cleared.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                continue;
            }

            history.Add(new ChatMessage(ChatRole.User, prompt));

            ChatOptions chatOptions = new()
            {
                MaxOutputTokens = _options.MaxOutputTokens,
                Temperature = _options.Temperature
            };

            StringBuilder answer = new();
            Console.Write("Assistant: ");

            try
            {
                await foreach (ChatResponseUpdate update in
                    chatClient.GetStreamingResponseAsync(
                        history,
                        chatOptions,
                        cancellationToken))
                {
                    if (string.IsNullOrEmpty(update.Text))
                    {
                        continue;
                    }

                    Console.Write(update.Text);
                    answer.Append(update.Text);
                }

                Console.WriteLine();
                history.Add(new ChatMessage(ChatRole.Assistant, answer.ToString()));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "The chat request failed");
                Console.WriteLine("\nThe request failed. See the logs for details.");
            }
        }

        logger.LogInformation("Chat application stopped");
    }
}
