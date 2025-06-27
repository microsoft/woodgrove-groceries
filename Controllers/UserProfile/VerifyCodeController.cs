using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using woodgrovedemo.Helpers;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VerifyCodeController : ControllerBase
{
    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;
    readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

    public VerifyCodeController(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
    {
        _configuration = configuration;
        _telemetry = telemetry;
        _authorizationHeaderProvider = authorizationHeaderProvider;
    }

    [HttpPost(Name = "VerifyCode")]
    public async Task<IActionResult> OnPostAsync([FromBody] VerifyCodeRequest request)
    {
        _telemetry.TrackPageView("Profile:VerifyCode");

        // Verify if MFA has been fulfilled
        bool StepUpFulfilled = User.Claims.Any(c => c.Type == "acrs" && c.Value == "c1");

        if (!StepUpFulfilled)
        {
            return Ok(new VerifyCodeResponse("Multi-factor authentication is required for this operation"));
        }

        // Read app settings
        string baseUrl = _configuration.GetSection("WoodgroveGroceriesApi:BaseUrl")!.Value!;
        string[] scopes = _configuration.GetSection("WoodgroveGroceriesApi:Scopes")!.Get<string[]>()!;
        string endpoint = _configuration.GetSection("WoodgroveGroceriesApi:Endpoint")!.Value! + "VerifyCode";

        // Check the scopes application settings
        if (scopes == null)
        {
            return Ok(new VerifyCodeResponse("The WoodgroveGroceriesApi:Scopes application setting is misconfigured or missing. Use the array format: [\"Account.Payment\", \"Account.Purchases\"]"));
        }

        // Check the base URL application settings
        if (string.IsNullOrEmpty(baseUrl))
        {
            return Ok(new VerifyCodeResponse("The WoodgroveGroceriesApi:BaseUrl application setting is misconfigured or missing. Check out your applications' scope base URL in Microsoft Entra admin center. For example: api://12345678-0000-0000-0000-000000000000"));
        }

        // Check the endpoint application settings
        if (string.IsNullOrEmpty(endpoint))
        {
            return Ok(new VerifyCodeResponse("The WoodgroveGroceriesApi:Endpoint application setting is misconfigured or missing."));
        }

        // Set the scope full URL (temporary workaround should be fix)
        for (int i = 0; i < scopes.Length; i++)
        {
            scopes[i] = $"{baseUrl}/{scopes[i]}";
        }

        string accessToken = string.Empty;

        try
        {
            // Get an access token to call the "Account" API (the first API in line)
            accessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(scopes);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
        {
            if (ex.MsalUiRequiredException.ErrorCode == "user_null")
            {
                return Ok(new VerifyCodeResponse("The token cache does not contain the token to access the web APIs. To get the access token, sign-out and sign-in again."));
            }
            else
            {
                return Ok(new VerifyCodeResponse("ex.MsalUiRequiredException.Message"));
            }
        }
        catch (System.Exception ex)
        {
            return Ok(new VerifyCodeResponse(ex.Message));
        }


        try
        {
            // Use the access token to call the Account API.
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);

                // Sent HTTP request to the web API endpoint
                var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.PostAsync(endpoint, content);

                // Ensure success and get the response
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                responseMessage.EnsureSuccessStatusCode();
                VerifyCodeResponse responseJson = VerifyCodeResponse.Parse(responseString);

                // Update the user's profile
                if (responseJson.ValidationPassed)
                {
                    if (responseJson.AuthType == AuthMethodType.SignInEmail)
                    {
                        // Update the sign-in email 
                        await UpdateSignInEmailAsync(responseJson);
                    }
                    else if (responseJson.AuthType == AuthMethodType.EmailMfa)
                    {
                        // Update the email MFA 
                        await UpdateEmailMfaAsync(responseJson);
                    }
                }

                return Ok(responseJson);
            }
        }
        catch (System.Exception ex)
        {
            return Ok(new VerifyCodeResponse(ex.Message));
        }
    }

    private async Task UpdateSignInEmailAsync(VerifyCodeResponse responseJson)
    {
        // Get the user unique identifier
        string? userObjectId = User.GetObjectId();

        try
        {
            var graphClient = MsalAccessTokenHandler.GetGraphClient(_configuration);

            User? profile = await graphClient.Users[User.GetObjectId()!].GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities" };
            });

            // Check if the user profile is null
            if (profile == null || profile.Identities == null)
            {
                throw new Exception("The user profile is not available or does not contain identities.");
            }

            // Find the identity with sign-in type "emailAddress" and update its IssuerAssignedId
            for (int i = 0; i < profile!.Identities!.Count; i++)
            {
                if (profile!.Identities![i].SignInType == "emailAddress")
                {
                    profile!.Identities![i].IssuerAssignedId = responseJson.AuthValue;
                    break;
                }
            }


            // Update user by object ID
            var requestBody = new User
            {
                Identities = profile!.Identities
            };


            var result = await graphClient.Users[userObjectId].PatchAsync(requestBody);
        }
        catch (ODataError odataError)
        {
            // TBD: Add UI error
            //ErrorMessage = $"The account cannot be updated due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "OnPostAsync");
        }
        catch (Exception ex)
        {
            // TBD: Add UI error
            //ErrorMessage = $"The account cannot be updated due to the following error: {ex.Message}";
            AppInsights.TrackException(_telemetry, ex, "OnPostAsync");
        }
    }

    private async Task UpdateEmailMfaAsync(VerifyCodeResponse responseJson)
    {
        // Get the user unique identifier
        string? userObjectId = User.GetObjectId();

        try
        {
            var graphClient = MsalAccessTokenHandler.GetGraphClient(_configuration);

            // Graph payload
            var requestBody = new EmailAuthenticationMethod
            {
                EmailAddress = responseJson.AuthValue,
            };

            // Get the current auth method
            var result = await graphClient.Users[userObjectId].Authentication.EmailMethods.GetAsync();

            if (result != null && result.Value != null && result.Value.Count > 0)
            {
                // Update the first email auth method
                string authMethodId = result.Value[0].Id!;
                var result1 = await graphClient.Users[userObjectId].Authentication.EmailMethods[authMethodId].PatchAsync(requestBody);
            }
            else
            {
                // Add a new email auth method
                var result1 = await graphClient.Users[userObjectId].Authentication.EmailMethods.PostAsync(requestBody);
            }

        }
        catch (ODataError odataError)
        {
            // TBD: Add UI error
            //ErrorMessage = $"The account cannot be updated due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "OnPostAsync");
        }
        catch (Exception ex)
        {
            // TBD: Add UI error
            //ErrorMessage = $"The account cannot be updated due to the following error: {ex.Message}";
            AppInsights.TrackException(_telemetry, ex, "OnPostAsync");
        }
    }
}
