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

            for (int i = 0; i < scopes.Length; i++)
            {
                scopes[i] = $"{baseUrl}/{scopes[i]}";
            }

            try
            {
                AccessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(scopes);
            }
            catch (MsalUiRequiredException ex)
            {
                AccessToken = "Error: " + ex.Message;
            }
            catch (System.Exception ex)
            {
                AccessToken = "Error: " + ex.Message;
            }

            return Page();
        }
    }
}
