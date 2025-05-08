using System.ClientModel;
using System.Diagnostics;
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

    public async Task SendMessage(string user, string prompt, string flow = "support")
    {
        switch (flow)
        {
            case "support":
                await SendSupportMessage(user, prompt);
                break;
            case "user":
                await SendUserMessage(user, prompt);
                break;
            default:
                await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", "Invalid flow type.");
                break;
        }
    }

    private async Task SendUserMessage(string user, string prompt)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string elapsedTime = "\nElapsed time: ";

        // Check if the user ID is the same as the one in the ID token
        // This is a security check to ensure that the user is who they say they are.
        if (!ValidRequest(user))
        {
            await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", "You are not authorized to send messages.");
            return;
        }

        // Inform the client that we are starting to process the message
        await Clients.Caller.SendAsync("ReceiveStartTyping", user, "Processing your message...");

        Response<Agent> agentResponse = null;
        Agent agent = null;

        try
        {
            agentResponse = await _agentsClient.GetAgentAsync(_configuration.GetSection("Demos:AzureOpenProject:WoodgroveAgentId").Value);
            agent = agentResponse.Value;

            // Add the elapsed time to the satistic message
            elapsedTime += "\nGetAgentAsync: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
        }
        catch (System.Exception)
        {

        }

        try
        {
            // If the agent does not exist, create it
            if (agent == null)
            {
                // Create a new agent
                agent = await _agentsClient.CreateAgentAsync(
                model: "gpt-4o-mini",
                name: "Woodgrove groceries agent",
                    instructions: "You are the Woogrove online retail store. Use the provided functions to help answer questions. "
                        + "Customize your responses to the user's preferences as much as possible and use friendly ",
                tools: [ChatTools.GetUserInfoDefinition]);

                // Add the elapsed time to the satistic message
                elapsedTime += "\nCreateAgentAsync: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            }

            Response<AgentThread> threadResponse = await _agentsClient.CreateThreadAsync();
            AgentThread thread = threadResponse.Value;

            // Add the elapsed time to the satistic message
            elapsedTime += "\nCreateThreadAsync: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

            Response<Azure.AI.Projects.ThreadMessage> messageResponse = await _agentsClient.CreateMessageAsync(
                thread.Id,
                MessageRole.User,
                prompt);

            // Add the elapsed time to the satistic message
            elapsedTime += "\nCreateMessageAsync: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

            List<ToolOutput> toolOutputs = [];
            ThreadRun streamRun = null;
            AsyncCollectionResult<StreamingUpdate> stream = _agentsClient.CreateRunStreamingAsync(thread.Id, agent.Id);

            // Add the elapsed time to the satistic message
            elapsedTime += "\nCreateRunStreamingAsync: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
            do
            {
                toolOutputs.Clear();
                await foreach (StreamingUpdate streamingUpdate in stream)
                {
                    if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
                    {
                        Console.WriteLine("--- Run started! ---");
                    }
                    else if (streamingUpdate is RequiredActionUpdate submitToolOutputsUpdate)
                    {
                        // Add the elapsed time to the satistic message
                        elapsedTime += "\nGetResolvedToolOutput started: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

                        // Call the corresponding function using the function name that required an action
                        RequiredActionUpdate newActionUpdate = submitToolOutputsUpdate;
                        var resolvedToolOutput = await ChatTools.GetResolvedToolOutput(
                            _configuration,
                            Context.User,
                            newActionUpdate.FunctionName,
                            newActionUpdate.ToolCallId,
                            user
                        //newActionUpdate.FunctionArguments
                        );

                        // Add the resolved tool output to the list of tool outputs
                        if (resolvedToolOutput != null)
                        {
                            toolOutputs.Add(resolvedToolOutput);
                        }

                        // Add the elapsed time to the satistic message
                        elapsedTime += "\nGetResolvedToolOutput completed: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

                        streamRun = submitToolOutputsUpdate.Value;
                    }
                    else if (streamingUpdate is MessageContentUpdate contentUpdate)
                    {
                        await Clients.Caller.SendAsync("ReceivePartialResponse", user, contentUpdate.Text);
                    }
                    else if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCompleted)
                    {
                        // Add the elapsed time to the satistic message
                        elapsedTime += "\nCompleted: " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

                        // Inform the client that we are done processing the message
                        await Clients.Caller.SendAsync("ReceiveEndTyping", user, "Done processing your message.", elapsedTime);
                    }
                }

                // If there are any tool outputs, submit them to the agent
                if (toolOutputs.Count > 0)
                {
                    stream = _agentsClient.SubmitToolOutputsToStreamAsync(streamRun, toolOutputs);
                }
            }
            while (toolOutputs.Count > 0);

            // Clean up the agent and thread
            await _agentsClient.DeleteThreadAsync(thread.Id);
            //await _agentsClient.DeleteAgentAsync(agent.Id);

        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", $"Error: {ex.Message}");
        }
    }


    private async Task SendSupportMessage(string user, string prompt)
    {
        // Check if the user ID is the same as the one in the ID token
        // This is a security check to ensure that the user is who they say they are.
        if (!ValidRequest(user))
        {
            await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", "You are not authorized to send messages.");
            return;
        }

        // Inform the client that we are starting to process the message
        await Clients.Caller.SendAsync("ReceiveStartTyping", user, "Processing your support question...");

        try
        {
            Response<Agent> agentResponse = await _agentsClient.GetAgentAsync(_configuration.GetSection("Demos:AzureOpenProject:SupportAgentId").Value);
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
                    await Clients.Caller.SendAsync("ReceivePartialResponse", user, contentUpdate.Text);
                }
            }

            await Clients.Caller.SendAsync("ReceiveEndTyping", user, "Done processing your message.");

        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", $"Error: {ex.Message}");
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
            await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", "You are not authorized to send messages.");
            return;
        }

        try
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, "Done resetting your conversation.");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveErrorMessage", "System", $"Error: {ex.Message}");
        }
    }
}