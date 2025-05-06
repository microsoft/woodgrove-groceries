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

namespace woodgrovedemo.Helpers.AzureAI;

public class ChatTools
{
    public static async Task<ToolOutput?> GetResolvedToolOutput(IConfiguration configuration, ClaimsPrincipal? claims, string functionName, string toolCallId, string functionArguments)
    {
        // Check the function name and tool call ID to determine which function to call
        if (functionName == GetUserInfoDefinition.Name)
        {
            return new ToolOutput(toolCallId, await GetUserInfoAsync(configuration, claims, functionArguments));
        }
        else
        {
            return new ToolOutput(toolCallId, $"Error: AI function {configuration} not found.");
        }
    }

    /// <summary>
    /// This method retrieves user account information, such as when the account was created, the last time the password was changed, and the last time the user signed in to the application.
    /// </summary>
    public static async Task<string> GetUserInfoAsync(IConfiguration configuration, ClaimsPrincipal? claims, string userObjectId)
    {
        try
        {
            // Get the extension attributes from the configuration
            string specialDietAttribute = configuration.GetSection("MicrosoftGraph:ExtensionAttributes").Value + "_SpecialDiet";

            // Get the user profile from Entra ID using the Graph API
            var graphClient = MsalAccessTokenHandler.GetGraphClient(configuration);
            User? profile = await graphClient.Users[userObjectId].GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities", "displayName", "GivenName", "Surname", "Country", "City", "AccountEnabled", "CreatedDateTime", "lastPasswordChangeDateTime", "signInActivity", specialDietAttribute };
                    requestConfiguration.QueryParameters.Expand = new string[] { "Extensions" };
                });

            // Check if the profile is null
            if (profile == null)
            {
                return $"Could not retrieve user profile for user ID: {userObjectId}. Please try again later.";
            }

            // Create a user profile object to store the information
            UserInfo userInfo = new UserInfo();
            userInfo.Id = profile.Id!.ToString();
            userInfo.DisplayName = profile.DisplayName;
            userInfo.Country = profile.Country;
            userInfo.City = profile.City;
            userInfo.CreatedDateTime = profile.CreatedDateTime?.ToString();
            userInfo.LastPasswordChangeDateTime = profile.LastPasswordChangeDateTime?.ToString();
            userInfo.LastSignInDateTime = profile.SignInActivity?.LastSignInDateTime?.ToString();

            // Get the special diet from the extension attributes
            object? specialDiet;

            if (profile.AdditionalData.TryGetValue(specialDietAttribute, out specialDiet))
            {
                userInfo.DietaryRestrictions = specialDiet.ToString();
            }

            // Get the user's claims from the ID token
            if (claims != null)
            {
                userInfo.LoyaltySince = claims.FindFirst("loyaltySince")?.Value;
                userInfo.LoyaltyTier = claims.FindFirst("loyaltyTier")?.Value;
                userInfo.LoyaltyNumber = claims.FindFirst("loyaltyNumber")?.Value;
                userInfo.acrs = claims.FindFirst("acr")?.Value;
            }

            return userInfo.ToString()!;

        }
        catch (ODataError odataError)
        {
            return $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            return $"Can't read the profile due to the following error: {error}";
        }
    }

    public static FunctionToolDefinition GetUserInfoDefinition = new(
        name: "GetUserInfo",
        description: @"Retrieves user profiles. 
                    The function returns a JSON object with profile information. Unavailable attributes will not be included.   
                    The JSON object contains the following user attributes:
                      **id** - The unique identifier of the user account.
                      **displayName** - The display name of the user account.
                      **country** - The country of the user account.
                      **city** - The city of the user account.
                      **createdDateTime** - The date and time when the user account was created.
                      **lastPasswordChangeDateTime** - The date and time when the password was last changed.
                      **lastSignInDateTime** - The date and time when the user last signed in to the application.
                      **DietaryRestrictions** - The dietary restrictions of the user account.
                      **LoyaltySince** - The date when the user joined the loyalty program.
                      **LoyaltyTier** - The loyalty tier of the user account.
                      **LoyaltyNumber** - The loyalty number of the user account.
                      **acrs** - The authentication context class reference (ACR) of the user account. If its value equals 'c1', it indicates that the multi-factor authentication (MFA) requirement is met. Otherwise, if the 'acrs' attribute doesn't exist or has another value, it means that user did not sign-in with MFA. Use this attribute to check if the user has passed MFA.
                      ");
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