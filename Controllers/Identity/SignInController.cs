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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("[controller]")]
public class SignInController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;
    readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

    public SignInController(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
    {
        _configuration = configuration;
        _telemetry = telemetry;
        _authorizationHeaderProvider = authorizationHeaderProvider;
    }

    public async Task<IActionResult> OnGetDefault(string handler, string? id = null)
    {
        // Retrieve the demo by the handler ID (case-insensitive).
        var demo = DemoDataList.Demos.FirstOrDefault(d => d.ID.Equals(handler, StringComparison.OrdinalIgnoreCase));
        if (demo == null)
        {
            // Track the error with its ID.
            _telemetry.TrackEvent("DemoNotFound", new Dictionary<string, string>
            {
                { "DemoID", handler }
            });

            // If the demo is not found, return NotFound.
            return NotFound();
        }

        // Track the demo with its ID.
        _telemetry.TrackPageView($"Sign-in:{demo.ID}");

        // If the demo requires a redirect, return a redirect to the demo URL.
        if (!string.IsNullOrEmpty(demo.RedirectUri))
        {
            return Redirect(demo.RedirectUri);
        }

        // If the demo is type of "act as", call the StartActAsAsync method.
        if (demo.ID == "ActAs")
        {
            await StartActAsAsync(handler);
        }

        ChallengeResult challengeResult = new ChallengeResult(
            OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                // Check if the demo has a post sign-in redirect URI. If not, set the default redirect URI to the root.
                RedirectUri = demo.PostSignInRedirectUri ?? "/",
            });

        // Fix the issue with the null
        if (challengeResult.Properties == null)
        {
            challengeResult.Properties = new AuthenticationProperties();
        }

        // Check if the demo has extra parameters.
        if (demo.ExtraParams != null && demo.ExtraParams.Count > 0)
        {
            // If the demo has extra parameters, add them to the query string.
            foreach (var param in demo.ExtraParams)
            {
                if (!string.IsNullOrEmpty(param.FixedValue))
                {
                    // If the parameter is static, add its value.
                    challengeResult.Properties.Items.Add(param.Name, param.FixedValue);
                }
                else if (!string.IsNullOrEmpty(param.QueryStringName))
                {
                    // If the parameter is dynamic, add its value from the query string.
                    challengeResult.Properties.Items.Add(param.Name, HttpContext.Request.Query[param.QueryStringName]);
                }
            }
        }

        // Check if reauth is required. If so, add the "force" parameter.
        if (demo.Reauth)
        {
            challengeResult.Properties.Items.Add("force", "true");
        }

        return challengeResult;
    }

    private async Task StartActAsAsync(string id)
    {
        // Input validation 
        if (id.Length > 20)
        {
            id = id.Substring(0, 20);
        }

        // Read app settings
        string baseUrl = _configuration.GetSection("WoodgroveGroceriesAuthApi:BaseUrl").Value!;
        string[] scopes = _configuration.GetSection("WoodgroveGroceriesAuthApi:Scopes")!.Get<string[]>()!;
        string endpoint = _configuration.GetSection("WoodgroveGroceriesAuthApi:Endpoint").Value! + "ActAsDemo";

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
        catch (MicrosoftIdentityWebChallengeUserException)
        {
            // TBD
        }
        catch (System.Exception)
        {
            // TBD
        }


        try
        {
            ActAsRequest actAsRequest = new ActAsRequest();
            actAsRequest.ActAs = id;

            // Use the access token to call the Account API.
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", accessToken);

                // Sent HTTP request to the web API endpoint
                var content = new StringContent(actAsRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = await client.PostAsync(endpoint, content);

                // Ensure success and get the response
                responseMessage.EnsureSuccessStatusCode();
                //string responseString = await responseMessage.Content.ReadAsStringAsync();
            }
        }
        catch (System.Exception)
        {
            // TBD
        }
    }
}
