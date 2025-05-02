using Microsoft.AspNetCore.SignalR;
using OpenAI.Assistants;

namespace woodgrovedemo.Helpers.AzureAI;

// This class is an endpoint for the SignalR hub that handles chat messages.
// The edpoint is protected by the "ExclusiveDemosOnly" authorization policy (defined in Startup.cs).
// It means that only users who are members of the exclusive demos security group can access this endpoint
public class ChatHub : Hub
{
    private readonly OpenAI.OpenAIClient _openAIClient;
    private readonly IConfiguration _configuration;

    public ChatHub(IConfiguration configuration, OpenAI.OpenAIClient openAIClient)
    {
        _openAIClient = openAIClient;
        _configuration = configuration;
    }

    public async Task SendMessage(string user, string prompt)
    {
        // Check if the user ID is the same as the one in the ID token
        // This is a security check to ensure that the user is who they say they are.
        if (!ValidRequest(user))
        {
            await Clients.All.SendAsync("ReceiveErrorMessage", "System", "You are not authorized to send messages.");
            return;
        }

        try
        {
            // Inform the client that we are starting to process the message
            await Clients.All.SendAsync("ReceiveStartTyping", user, "Processing your message...");

            // Create an assistant client to manage the assistant and its threads
            AssistantClient assistantClient = _openAIClient.GetAssistantClient();

            // Create assistant options with request details to use when creating a new assistant in the next step
            AssistantCreationOptions assistantOptions = new AssistantCreationOptions
            {
                Name = "Woodgrove Assistant",
                Instructions = "You are an AI assistant that helps people find information."
            };

            // Create the assistant with a model and the assistant options
            // The assistant manages conversation threads, and can utilize 'tools'.
            Assistant assistant = await assistantClient.CreateAssistantAsync(
                _configuration.GetSection("Demos:AzureOpenAI:DeploymentName").Value!,
                assistantOptions);

            // Create a thread options with the initial message
            // The thread options are used to create a new thread in the next step
            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { prompt }
            };

            // Create a thread but don't run it yet
            AssistantThread thread = await assistantClient.CreateThreadAsync(threadOptions);

            // Create a run options with the assistant ID
            RunCreationOptions runOptions = new RunCreationOptions();

            // Create and start the run
            var streamingUpdates = assistantClient.CreateRunStreamingAsync(
                thread.Id,
                assistant.Id,
                runOptions
            );

            // Stream the response
            await foreach (var update in streamingUpdates)
            {
                if (update is MessageContentUpdate)
                {
                    await Clients.All.SendAsync("ReceivePartialResponse", user, ((MessageContentUpdate)update).Text);
                }
            }

            // Cleanup
            await assistantClient.DeleteThreadAsync(thread.Id);
            await assistantClient.DeleteAssistantAsync(assistant.Id);

            await Clients.All.SendAsync("ReceiveEndTyping", user, "Done processing your message.");
        }
        catch (Exception ex)
        {
            await Clients.All.SendAsync("ReceiveErrorMessage", "System", $"Error: {ex.Message}");
        }
    }

    private bool ValidRequest(string user)
    {
        // Get the user ID from the ID token
        string tokenUserID = Context.User?.Claims?.FirstOrDefault(c => c.Type.ToLower() == "oid")?.Value ?? string.Empty;
        
        // Check if the user ID is the same as the one in the ID token
        return (string.IsNullOrEmpty(tokenUserID) != true && tokenUserID == user);

    }


    public async Task ResetConversation(string user)
    {
        // Check if the user ID is the same as the one in the ID token
        // This is a security check to ensure that the user is who they say they are.
        if (!ValidRequest(user))
        {
            await Clients.All.SendAsync("ReceiveErrorMessage", "System", "You are not authorized to send messages.");
            return;
        }

        try
        {
            await Clients.All.SendAsync("ReceiveMessage", user, "Done resetting your conversation.");
        }
        catch (Exception ex)
        {
            await Clients.All.SendAsync("ReceiveErrorMessage", "System", $"Error: {ex.Message}");
        }
    }
}
