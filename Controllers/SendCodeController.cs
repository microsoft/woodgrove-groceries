using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class SendCodeController : ControllerBase
{
    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;
    readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

    public SendCodeController(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
    {
        _configuration = configuration;
        _telemetry = telemetry;
        _authorizationHeaderProvider = authorizationHeaderProvider;
    }

    [HttpPost(Name = "SendCode")]
    public async Task<IActionResult> OnPostAsync([FromBody] SendCodeRequest request)
    {
        _telemetry.TrackPageView("Profile:SendCode");

        // Read app settings
        string baseUrl = _configuration.GetSection("WoodgroveGroceriesApi:BaseUrl").Value!;
        string[] scopes = _configuration.GetSection("WoodgroveGroceriesApi:Scopes").Get<string[]>();
        string endpoint = _configuration.GetSection("WoodgroveGroceriesApi:Endpoint").Value! + "SendCode";

        // Check the scopes application settings
        if (scopes == null)
        {
            return Ok(new SendCodeResponse("The WoodgroveGroceriesApi:Scopes application setting is misconfigured or missing. Use the array format: [\"Account.Payment\", \"Account.Purchases\"]"));
        }

        // Check the base URL application settings
        if (string.IsNullOrEmpty(baseUrl))
        {
            return Ok(new SendCodeResponse("The WoodgroveGroceriesApi:BaseUrl application setting is misconfigured or missing. Check out your applications' scope base URL in Microsoft Entra admin center. For example: api://12345678-0000-0000-0000-000000000000"));
        }

        // Check the endpoint application settings
        if (string.IsNullOrEmpty(endpoint))
        {
            return Ok(new SendCodeResponse("The WoodgroveGroceriesApi:Endpoint application setting is misconfigured or missing."));
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
                return Ok(new SendCodeResponse("The token cache does not contain the token to access the web APIs. To get the access token, sign-out and sign-in again."));
            }
            else
            {
                return Ok(new SendCodeResponse("ex.MsalUiRequiredException.Message"));
            }
        }
        catch (System.Exception ex)
        {
            return Ok(new SendCodeResponse(ex.Message));
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
                responseMessage.EnsureSuccessStatusCode();
                string responseString = await responseMessage.Content.ReadAsStringAsync();
                SendCodeResponse responseJson = SendCodeResponse.Parse(responseString);

                return Ok(responseJson);
            }
        }
        catch (System.Exception ex)
        {
            return Ok(new SendCodeResponse(ex.Message));
        }
    }
}
