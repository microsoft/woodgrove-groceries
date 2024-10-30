using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class SignOutModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public SignOutModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        //public async Task OnGetAsync()
        public IActionResult OnGet()
        {

            _telemetry.TrackPageView("SignOut");

            string? scheme = User.Claims.FirstOrDefault(c => c.Type.ToLower() == "scheme")?.Value;

            // Loop through all cookies in the response
            if (scheme != OpenIdConnectDefaults.AuthenticationScheme)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
            }

            return SignOut( scheme ??= OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
