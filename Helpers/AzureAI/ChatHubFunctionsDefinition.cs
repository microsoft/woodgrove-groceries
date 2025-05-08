using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.AI.Projects;


namespace woodgrovedemo.Helpers.AzureAI;

public class ChatHubFunctionsDefinition
{
    public static FunctionToolDefinition GetUserInfo = new(
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
                      **acrs** - The authentication context class reference (ACR) of the user account. If its value equals 'c1', it indicates that the multi-factor authentication (MFA) requirement is met. Otherwise, if the 'acrs' attribute doesn't exist or has another value, it means that user did not sign-in with MFA. Use this attribute to check if the user has passed MFA.");

    public static FunctionToolDefinition UpdateUserProfile = new(
            name: "UpdateUserInfo",
            description: "Updates the user profile with the provided information.",
            parameters: BinaryData.FromObjectAsJson(
                new
                {
                    Type = "object",
                    Properties = new
                    {
                        Attribute = new
                        {
                            Type = "string",
                            Description = @"The name of the attribute to update. Possible values are: 
                              - **displayName** - The display name of the user account.
                              - **country** - The country of the user account.
                              - **city** - The city of the user account.
                            Attributes that are not listed here are not supported."
                        },
                        Value = new
                        {
                            Type = "string",
                            Description = @"The new value for the attribute."
                        }
                    },
                    Required = new[] { "attribute" },
                },
                new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
}
