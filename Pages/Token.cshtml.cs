using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;

namespace woodgrovedemo.Pages
{
    [Authorize]
    public class TokenModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private TelemetryClient _telemetry;
        readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
        public string AccessToken { get; set; }
        public string AccessTokenError { get; set; }

        public TokenModel(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            _configuration = configuration;
            _telemetry = telemetry;
            _authorizationHeaderProvider = authorizationHeaderProvider;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _telemetry.TrackPageView("Token");


            // Acquire the access token.
            string baseUrl = _configuration.GetSection("MyApi:BaseUrl").Value!;
            string[] scopes = _configuration.GetSection("MyApi:Scopes").Get<string[]>();

            if (scopes == null)
            {
                AccessTokenError = "Error: The MyApi:Scopes application setting is misconfigured or missing. Use the array format: [\"Account.Payment\", \"Account.Purchases\"]";
            }
            else if (baseUrl == null)
            {
                AccessTokenError = "Error: The MyApi:BaseUrl application setting is misconfigured or missing. Check out your applications' scope base URL in Microsoft Entra admin center. For example: api://12345678-0000-0000-0000-000000000000";
            }
            else
            {
                for (int i = 0; i < scopes.Length; i++)
                {
                    scopes[i] = $"{baseUrl}/{scopes[i]}";
                }

                try
                {
                    AccessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(scopes);
                }
                catch (System.Exception ex)
                {
                    if (ex.GetType().ToString().Contains("MicrosoftIdentityWebChallengeUserException"))
                    {
                        AccessTokenError = "Error: The token cache does not contain the token to access the web APIs. To get the access token, sign-out and sign-in again.";
                    }
                    else
                    {
                        AccessTokenError = "Error: " + ex.Message;
                    }

                }
            }

            return Page();
        }
    }
}
