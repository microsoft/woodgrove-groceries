using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Abstractions;

namespace woodgrovedemo.Helpers.AzureAI;

/// <summary>
/// This class contains the implementation of the AI functions that are called by the Azure OpenAI service.
/// The AI functions are defined in the ChatHubFunctionsDefinition class, and this class implements the logic for those functions.
/// It's a partial class, so it can be extended the ChatHub.cs files.
/// </summary>
public partial class ChatHub
{
    public async Task<ToolOutput?> GetResolvedToolOutput(string functionName, string toolCallId, string functionArguments)
    {
        // Check the function name and tool call ID to determine which function to call
        if (functionName == ChatHubFunctionsDefinition.GetUserInfo.Name)
        {
            return new ToolOutput(toolCallId, await GetUserInfoAsync());
        }
        else
        {
            return new ToolOutput(toolCallId, $"Error: AI function {functionName} not found.");
        }
    }

    /// <summary>
    /// This method retrieves user account information, such as when the account was created, the last time the password was changed, and the last time the user signed in to the application.
    /// </summary>
    public async Task<string> GetUserInfoAsync()
    {
        try
        {
            // Use the user's access token to authenticate the GraphServiceClient and call the Graph API 'me' endpoint
            string specialDietAttribute = _configuration.GetSection("MicrosoftGraph:ExtensionAttributes").Value + "_SpecialDiet";

            User? profile = await _graphServiceClient.Me.GetAsync(requestConfiguration =>
                {
                    // Select the attributes to retrieve from the Graph API
                    requestConfiguration.QueryParameters.Select = new string[] { "Id",
                                "displayName", "GivenName", "Surname",
                                "Country", "City",
                                "identities", "AccountEnabled",
                                "CreatedDateTime", "lastPasswordChangeDateTime",
                                 specialDietAttribute
                                 /*"signInActivity",*/ };

                    // Expand the Extensions property to retrieve the extension attributes
                    requestConfiguration.QueryParameters.Expand = new string[] { "Extensions" };
                });

            // Check if the profile is null
            if (profile == null)
            {
                return $"Could not retrieve your profile. Please try again later.";
            }

            // Create a user profile object to store the information
            UserInfo userInfo = new UserInfo();
            userInfo.Id = profile.Id!.ToString();
            userInfo.DisplayName = profile.DisplayName;
            userInfo.Country = profile.Country;
            userInfo.City = profile.City;
            userInfo.CreatedDateTime = profile.CreatedDateTime?.ToString();
            userInfo.LastPasswordChangeDateTime = profile.LastPasswordChangeDateTime?.ToString();

            //userInfo.LastSignInDateTime = profile.SignInActivity?.LastSignInDateTime?.ToString();

            // Get the special diet from the extension attributes
            object? specialDiet;

            if (profile.AdditionalData.TryGetValue(specialDietAttribute, out specialDiet))
            {
                userInfo.DietaryRestrictions = specialDiet.ToString();
            }

            // Get the user's claims from the ID token
            if (Context.User != null)
            {
                userInfo.LoyaltySince = Context.User.FindFirst("loyaltySince")?.Value;
                userInfo.LoyaltyTier = Context.User.FindFirst("loyaltyTier")?.Value;
                userInfo.LoyaltyNumber = Context.User.FindFirst("loyaltyNumber")?.Value;
                userInfo.acrs = Context.User.FindFirst("acr")?.Value;
            }

            return userInfo.ToString()!;

        }
        catch (ODataError ex)
        {
            string error = $"Can't read the profile due to the following error: {ex.Error!.Message} Error code: {ex.Error.Code}";
            await Clients.Caller.SendAsync("ReceiveErrorMessage", error);
            return $"Can't read the profile due to the following error: {error}";
        }
        catch (Exception ex)
        {
            string error = ex.Message;

            // If the inner exception is available, include it in the error message
            if (ex.InnerException != null)
            {
                error += $" Inner exception: {ex.InnerException.Message}";
            }

            // Retrun the error message to the client
            await Clients.Caller.SendAsync("ReceiveErrorMessage", error);
            return $"Can't read the profile due to the following error: {error}";
        }
    }
}

public class UserInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DisplayName { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Country { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? City { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CreatedDateTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastPasswordChangeDateTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastSignInDateTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DietaryRestrictions { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LoyaltySince { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LoyaltyTier { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LoyaltyNumber { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? acrs { get; set; }

    /// <summary>
    /// Serialize this object into a JSON string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}