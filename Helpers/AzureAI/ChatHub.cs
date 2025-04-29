using Microsoft.AspNetCore.SignalR;
using OpenAI.Chat;

namespace woodgrovedemo.Helpers.AzureAI;

// This class is an endpoint for the SignalR hub that handles chat messages.
public class ChatHub : Hub
{
    private readonly OpenAI.OpenAIClient _openAIClient;
    private readonly IConfiguration _configuration;

    public ChatHub(IConfiguration configuration, OpenAI.OpenAIClient openAIClient)
    {
        _openAIClient = openAIClient;
        _configuration = configuration;
    }

    public async Task SendMessage(string user, string message)
    {
        var chatClient = _openAIClient.GetChatClient(_configuration.GetSection("Demos:AzureOpenAI:DeploymentName").Value!);

        // Create a chat comoletion options object
        ChatCompletionOptions options = new ChatCompletionOptions();
        options.MaxOutputTokenCount = 1000; // Set the maximum number of tokens in the response
        options.Temperature = 0.7f; // Set the temperature for randomness in the response
        options.TopP = 0.95f; // Set the top-p sampling parameter   

        // Create a list of chat messages
        // The first message is a system message that sets the context for the assistant
        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage("You are an AI assistant that helps people find information."),
            new UserChatMessage(message),
        };

        try
        {
            ChatCompletion chatCompletion = await chatClient.CompleteChatAsync(messages, options);

            // Get the assistant's response
            string response = chatCompletion.Content[0].Text;

            // Send the response back to the connected clients (using the user ID)
            await Clients.All.SendAsync("ReceiveMessage", user, response);
        }
        catch (Exception ex)
        {
            // Log the error and send an error message back to the client (using the user ID)
            await Clients.All.SendAsync("ReceiveMessage", user, $"Error processing message: {ex.Message}");
        }
    }
}
