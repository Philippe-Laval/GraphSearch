using Microsoft.Extensions.Options;

namespace GraphRag.Core.Configuration;

public sealed class AiOptionsValidator : IValidateOptions<AiOptions>
{
    public ValidateOptionsResult Validate(string? name, AiOptions options)
    {
        List<string> errors = [];

        switch (options.Provider.Trim())
        {
            case AiProviders.OpenAI:
                Require(options.OpenAI.ApiKey, "AI:OpenAI:ApiKey is required.", errors);
                Require(options.OpenAI.ChatModel, "AI:OpenAI:ChatModel is required.", errors);
                break;

            case AiProviders.AzureOpenAI:
                if (options.AzureOpenAI.Endpoint is null)
                {
                    errors.Add("AI:AzureOpenAI:Endpoint is required.");
                }

                Require(options.AzureOpenAI.ApiKey, "AI:AzureOpenAI:ApiKey is required.", errors);
                Require(options.AzureOpenAI.ChatDeploymentName,
                    "AI:AzureOpenAI:ChatDeploymentName is required.", errors);
                break;

            case AiProviders.Ollama:
                Require(options.Ollama.ChatModel, "AI:Ollama:ChatModel is required.", errors);
                break;

            default:
                errors.Add(
                    $"AI:Provider must be one of: {AiProviders.OpenAI}, " +
                    $"{AiProviders.AzureOpenAI}, {AiProviders.Ollama}.");
                break;
        }

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }

    private static void Require(string? value, string message, ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(message);
        }
    }
}
