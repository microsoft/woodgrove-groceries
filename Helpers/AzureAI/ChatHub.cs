using Azure;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.AspNetCore.SignalR;

namespace woodgrovedemo.Helpers.AzureAI;

// This class is an endpoint for the SignalR hub that handles chat messages.
// The edpoint is protected by the "ExclusiveDemosOnly" authorization policy (defined in Startup.cs).
// It means that only users who are members of the exclusive demos security group can access this endpoint
public class ChatHub : Hub
{
    //private readonly OpenAI.OpenAIClient _openAIClient;
    private readonly AgentsClient _agentsClient;
    private readonly IConfiguration _configuration;

    public ChatHub(IConfiguration configuration, AgentsClient agentsClient)
    {
        _agentsClient = agentsClient;
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

        // Inform the client that we are starting to process the message
        await Clients.All.SendAsync("ReceiveStartTyping", user, "Processing your message...");

        try
        {
            Response<Agent> agentResponse = await _agentsClient.GetAgentAsync(_configuration.GetSection("Demos:AzureOpenProject:AgentId").Value);
            Agent agent = agentResponse.Value;

            Response<AgentThread> threadResponse = await _agentsClient.CreateThreadAsync();
            AgentThread thread = threadResponse.Value;

            Response<Azure.AI.Projects.ThreadMessage> messageResponse = await _agentsClient.CreateMessageAsync(
                thread.Id,
                MessageRole.User,
                prompt);
            

            // This code is based on the Azure OpenAI SDK for .NET sample https://github.com/Azure/azure-sdk-for-net/blob/Azure.AI.Projects_1.0.0-beta.8/sdk/ai/Azure.AI.Projects/tests/Samples/Agent/Sample_Agent_Streaming.cs
            await foreach (StreamingUpdate streamingUpdate in _agentsClient.CreateRunStreamingAsync(thread.Id, agent.Id))
            {
                if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
                {
                    Console.WriteLine($"--- Run started! ---");
                }
                else if (streamingUpdate is MessageContentUpdate contentUpdate)
                {
                    //Console.Write(contentUpdate.Text);
                    await Clients.All.SendAsync("ReceivePartialResponse", user, contentUpdate.Text);
                }
            }

            // ThreadMessage message = messageResponse.Value;

            // Response<ThreadRun> runResponse = await client.CreateRunAsync(
            //     thread.Id,
            //     agent.Id);
            // ThreadRun run = runResponse.Value;

            // // Poll until the run reaches a terminal status
            // do
            // {
            //     await Task.Delay(TimeSpan.FromMilliseconds(500));
            //     runResponse = await client.GetRunAsync(thread.Id, runResponse.Value.Id);
            // }
            // while (runResponse.Value.Status == RunStatus.Queued
            //     || runResponse.Value.Status == RunStatus.InProgress);

            // Response<PageableList<ThreadMessage>> messagesResponse = await client.GetMessagesAsync(thread.Id);
            // IReadOnlyList<ThreadMessage> messages = messagesResponse.Value.Data;

            // // Display messages
            // foreach (ThreadMessage threadMessage in messages)
            // {
            //     //Console.Write($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
            //     foreach (MessageContent contentItem in threadMessage.ContentItems)
            //     {
            //         if (contentItem is MessageTextContent textItem)
            //         {
            //             Console.Write(textItem.Text);
            //             await Clients.All.SendAsync("ReceivePartialResponse", user, textItem.Text);
            //         }
            //     }
            //     Console.WriteLine();
            // }


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