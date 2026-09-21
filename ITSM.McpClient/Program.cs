Console.WriteLine("Hello, World!");

// https://deepwiki.com/donaldmucci/mcp-csharp-sdk/3.3-creating-and-exposing-prompts

/*
// List all available prompts
var prompts = await client.ListPromptsAsync(cancellationToken);

// Get a specific prompt by name
var prompt = prompts.First(p => p.Name == "GreetingPrompt");

// Get a prompt with arguments
var result = await prompt.GetAsync(
    new Dictionary<string, object?> { ["name"] = "Alice" },
    cancellationToken);

// Get chat messages from the result
var chatMessages = result.ToChatMessages();

await using (client.RegisterNotificationHandler(
    "notifications/prompts/list_changed", 
    (notification, cancellationToken) => {
        // Handle the notification
        return default;
    }))
{
    // Do work while listening for prompt changes
}


// For server errors or missing prompts
try {
    var promptResult = await client.GetPromptAsync("UnknownPrompt", cancellationToken);
} catch (McpException ex) {
    // Handle the error (ex.ErrorCode contains more details)
}
 
// For prompt implementation errors
try {
    var promptResult = await client.GetPromptAsync("PromptThatThrows", cancellationToken);
} catch (McpException ex) {
    // Handle the error
}

*/