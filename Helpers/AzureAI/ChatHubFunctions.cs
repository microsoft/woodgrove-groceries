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
        else if (functionName == ChatHubFunctionsDefinition.UpdateUserProfile.Name)
        {
            return new ToolOutput(toolCallId, await UpdateUserInfoAsync(functionArguments));
        }
        else
        {
            return new ToolOutput(toolCallId, $"Error: AI function {functionName} not found.");
        }
    }

    public async Task<string> UpdateUserInfoAsync(string functionArguments)
    {
        // Deserialize the function arguments into a dynamic object
        var functionArgs = JsonSerializer.Deserialize<Dictionary<string, string>>(functionArguments);

        // Check if the attribute and value are provided in the function arguments
        if (functionArgs == null || functionArgs.ContainsKey("attribute") == false || functionArgs.ContainsKey("value") == false)
        {
            return "Unable to update your profile. Please specify the attribute you want to update and its new value. The supported attributes are display name, city, and country. For example, you may request: 'Please update my name to Ellena'.";
        }

        // Get the attribute and value from the function arguments
        string attribute = functionArgs["attribute"];
        string value = functionArgs["value"];

        // Check if the attribute the following collection displayName, city, or country
        if (attribute == null || value == null || (attribute != "displayName" && attribute != "city" && attribute != "country"))
        {
            return "Unable to update your profile. Please specify the attribute you want to update and its new value. The supported attributes are display name, city, and country. For example, you may request: 'Please update my name to Ellena'.";
        }

        // Check the attribute name and value to determine which attribute to update
        var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>(attribute, value),
                    new KeyValuePair<string, string>("dontSkipEmptyString", false.ToString()),
                });


        // Read app settings
        string baseUrl = _configuration.GetSection("GraphApiMiddleware:BaseUrl").Value!;
        string[] scopes = _configuration.GetSection("GraphApiMiddleware:Scopes").Get<string[]>() ?? Array.Empty<string>();
        string endpoint = _configuration.GetSection("GraphApiMiddleware:Endpoint").Value!;

        // Check all of the app settings and return an error message if any of them is missing 
        if (baseUrl == null || scopes.Length == 0 || string.IsNullOrEmpty(endpoint))
        {
            return "We are currently unable to access your profile due to an internal issue. Please try again later.";
        }

        // Set the scope full URL (temporary workaround should be fix)
        for (int i = 0; i < scopes.Length; i++)
        {
            scopes[i] = $"{baseUrl}/{scopes[i]}";
        }

        try
        {
            // Get an access token to call the Graph middleware API
            string accessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(scopes);

            // Use the access token to call the Graph middleware API.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", accessToken);
            var httpResponseMessage = await client.PostAsync(endpoint, formContent);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            // Try to parse the response content as a MiddlewareApiResponse object
            MiddlewareApiResponse? middlewareApiResponse = MiddlewareApiResponse.Parse(responseContent);

            // Check if the response contains an error message
            if (middlewareApiResponse != null && string.IsNullOrEmpty(middlewareApiResponse.errorMessage) == false)
            {
                // Return the error message to the client
                await Clients.Caller.SendAsync("ReceiveErrorMessage", middlewareApiResponse.errorMessage);
                return $"Unable to update your profile. Please try again later.";
            }

            return $"Your profile has been updated successfully. Attribute: {attribute}, Value: {value}. Changes may take a few minutes to appear. Wait a few seconds, then [sign in again.](/SignIn?handler=ProfileReauth)";
        }
        catch (Exception ex)
        {
            string error = ex.Message;

            // If the inner exception is available, include it in the error message
            if (ex.InnerException != null)
            {
                error += $" Inner exception: {ex.InnerException.Message}";
            }


            // Check if the error message is related to the multi-factor authentication (MFA) requirement
            if (error.Contains("AADSTS50076") || error.Contains("multi-factor authentication"))
            {
                error = "(AADSTS50076) Your profile cannot be updated because you need to complete the multi-factor authentication (MFA) process. Please [sign in](/SignIn?handler=ChatMfa) again to complete the MFA process.";
            }

            // Retrun the error message to the client
            await Clients.Caller.SendAsync("ReceiveErrorMessage", error);
            return $"Can't read the profile due to the following error: {error}";
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

                // The 'acrs' claim (string collection) is used to verify whether the user has fulfilled the multi-factor authentication (MFA) requirement. 
                // If one its values is c1, it indicates that the MFA requirement has been satisfied. 
                userInfo.MFA = Context.User.Claims.Any(c => c.Type == "acrs" && c.Value == "c1");

                // Check if the user is a member of the commercial accounts security group
                string? commercialAccountsSecurityGroup = _configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value;
                if (commercialAccountsSecurityGroup != null)
                {
                    userInfo.CommercialAccount = Context.User.Claims.Any(c => c.Type == "groups" && c.Value == commercialAccountsSecurityGroup);
                }

                // Check if the user has the permissions to manage orders
                userInfo.CanManageOrders = Context.User.IsInRole("Orders.Manager");

                // Check if the user has the permissions to manage products
                userInfo.CanManageProducts = Context.User.IsInRole("Products.Contributor");
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
    public bool? MFA { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CommercialAccount { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CanManageOrders { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CanManageProducts { get; set; }

    /// <summary>
    /// Serialize this object into a JSON string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
    }
}

public class MiddlewareApiResponse
{
    public string? errorMessage { get; set; }

    public static MiddlewareApiResponse? Parse(string JsonString)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<MiddlewareApiResponse>(JsonString, options);
    }
}