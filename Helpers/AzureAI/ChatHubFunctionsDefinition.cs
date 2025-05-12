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
                      - **id** - The unique identifier of the user account.
                      - **displayName** - The display name of the user account.
                      - **country** - The country of the user account.
                      - **city** - The city of the user account.
                      - **createdDateTime** - The date and time when the user account was created.
                      - **lastPasswordChangeDateTime** - The date and time when the password was last changed.
                      - **lastSignInDateTime** - The date and time when the user last signed in to the application.
                      - **DietaryRestrictions** - The dietary restrictions of the user account.
                      - **LoyaltySince** - The date when the user joined the loyalty program.
                      - **LoyaltyTier** - The loyalty tier of the user account.
                      - **LoyaltyNumber** - The loyalty number of the user account.
                      - **MFA** - Multi-factor authentication (MFA) attribute. If the 'MFA' is true, it means that the MFA requirement has been met. If the MFA attribute is false, it indicates that the user did not sign in with MFA. This attribute can be used to verify whether the user has satisfied the MFA requirement.
                      - **CommercialAccount** - The ommercial account attribute. If the 'CommercialAccount' is true, it indicates that the user is a commercial account. As a result, the user is eligible for discounts with special prices.
                      - **CanManageOrders** - The 'CanManageOrders' is a Boolean attribute that indicates whether the user hat the permissions to manage Woodgrove's online orders. If true, users can manage orders; if false, they cannot
                      - **CanManageProducts** - The 'CanManageProducts' is a Boolean attribute that indicates whether the user hat the permissions to manage Woodgrove's products. If true, users can manage products; if false, they cannot");

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
