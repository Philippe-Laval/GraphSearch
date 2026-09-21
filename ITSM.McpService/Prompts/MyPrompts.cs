using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;

/*
Supported Return Types
Prompts can return various types which will be automatically converted to the appropriate format:

Return Type	Description
GetPromptResult	Direct prompt result, used as-is
string	Converted to a single user message
PromptMessage	A single prompt message
IEnumerable<PromptMessage>	Multiple prompt messages
ChatMessage	Converted to prompt message
IEnumerable<ChatMessage>	Converted to multiple prompt messages
*/

namespace ITSM.McpService.Prompts
{
    [McpServerPromptType]
    public sealed class MyPrompts
    {
        [McpServerPrompt, Description("A simple greeting prompt")]
        public static ChatMessage[] GreetingPrompt([Description("The name to greet")] string name) =>
            [
                new(ChatRole.User, $"Hello, {name}!"),
                new(ChatRole.User, "Please introduce yourself briefly."),
            ];
    }

    /*
    [McpServerPromptType]
    public sealed class DatabasePrompts(DatabaseService dbService)
    {
        [McpServerPrompt]
        public ChatMessage[] GetUserInfoPrompt(string userId) =>
            [
                new(ChatRole.User, $"User information: {dbService.GetUserInfo(userId)}"),
                new(ChatRole.User, "Please use this information to personalize your response."),
            ];
    }
    */
}
