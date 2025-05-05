using System.Threading.Tasks;
using Azure;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace woodgrovedemo.Helpers.AzureAI;

public class ChatTools
{
    public static async Task<ToolOutput?> GetResolvedToolOutput(IConfiguration configuration, string functionName, string toolCallId, string functionArguments)
    {
        // Check the function name and tool call ID to determine which function to call
        if (functionName == GetUserInfoDefinition.Name)
        {
            return new ToolOutput(toolCallId, await GetUserInfoAsync(configuration, functionArguments));
        }
        else if (functionName == GetLastSignInInfoDefinition.Name)
        {
            return new ToolOutput(toolCallId, await GetLastSignInInfoAsync(configuration, functionArguments));
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// This method retrieves user account information, such as when the account was created, the last time the password was changed, and the last time the user signed in to the application.
    /// </summary>
    public static async Task<string> GetUserInfoAsync(IConfiguration configuration, string userObjectId)
    {
        try
        {
            // Get the user porfile from the Graph API
            User? profile = await GetUserProfileAsync(configuration, userObjectId);

            // Get the date when the user account was created
            if (profile != null && profile.LastPasswordChangeDateTime != null)
            {
                return profile.LastPasswordChangeDateTime.ToString()!;
            }
            else
            {
                return "Could not retrieve your data. Please try again later.";
            }
        }
        catch (Exception ex)
        {
            return (ex.Message);
        }
    }

    public static FunctionToolDefinition GetUserInfoDefinition = new(
        name: "GetUserInfo",
        description: "Retrieves the date when the user account was created"
    );


    /// <summary>
    /// This method retrieves the last time user signed in to the application.
    /// </summary>
    public static async Task<string> GetLastSignInInfoAsync(IConfiguration configuration, string userObjectId)
    {
        try
        {
            // Get the user porfile from the Graph API
            User? profile = await GetUserProfileAsync(configuration, userObjectId);

            // Get the sign-in activity
            if (profile != null && profile.SignInActivity != null)
            {
                return profile.SignInActivity.LastSignInDateTime.ToString()!;
            }
            else
            {
                return "Could not retrieve your data. Please try again later.";
            }
        }
        catch (Exception ex)
        {
            return (ex.Message);
        }
    }

    public static FunctionToolDefinition GetLastSignInInfoDefinition = new(
        name: "GetLastSignInInfo",
        description: "Retrieves information regarding the user's most recent login, including the date and time of their last sign-in.");


    /// <summary>
    /// This method retrieves the user profile from the Graph API using the Microsoft Graph SDK.
    /// </summary>
    private static async Task<User?> GetUserProfileAsync(IConfiguration configuration, string userObjectId)
    {

        var graphClient = MsalAccessTokenHandler.GetGraphClient(configuration);

        // This method should return the user information from the Graph API
        try
        {
            User? profile = await graphClient.Users[userObjectId].GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities", "displayName", "GivenName", "Surname", "Country", "City", "AccountEnabled", "CreatedDateTime", "lastPasswordChangeDateTime", "signInActivity" };
                    //requestConfiguration.QueryParameters.Expand = new string[] { "Extensions" };
                });

            if (profile == null)
            {
                throw new Exception($"Could not retrieve user profile for user ID: {userObjectId}.");
            }

            return profile;
        }
        catch (ODataError odataError)
        {
            throw new Exception($"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}");
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            throw new Exception($"Can't read the profile due to the following error: {error}");
        }
    }
}