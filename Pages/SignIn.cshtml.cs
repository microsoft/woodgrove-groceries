using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{

    [AllowAnonymous]
    public class SignInModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public SignInModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        private IActionResult TrackAndAuth(string ID, string redirectUri = "/")
        {
            _telemetry.TrackPageView($"Sign-in:{ID}");

            return new ChallengeResult(
                OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                });
        }


        public IActionResult OnGetDefault()
        {
            return this.TrackAndAuth("Default");
        }

        public IActionResult OnGetOnlineRetail()
        {
            return this.TrackAndAuth("OnlineRetail");
        }

        public IActionResult OnGetProfileSignin()
        {
            return this.TrackAndAuth("ProfileSignin");
        }

        public IActionResult OnGetTokenSignin()
        {
            return this.TrackAndAuth("TokenSignin", "/token");
        }

        public IActionResult OnGetEmailAndPassword()
        {
            return this.TrackAndAuth("EmailAndPassword");
        }

        public IActionResult OnGetSocial()
        {
            return this.TrackAndAuth("Social");
        }

        public IActionResult OnGetTokenAugmentation()
        {
            return this.TrackAndAuth("TokenAugmentation");
        }

        public IActionResult OnGetPostAttributeCollection()
        {
            return this.TrackAndAuth("PostAttributeCollection");
        }

        public IActionResult OnGetRBAC()
        {
            return this.TrackAndAuth("RBAC");
        }

        public IActionResult OnGetSSPR()
        {
            return this.TrackAndAuth("SSPR");
        }

        public IActionResult OnGetKiosk()
        {
            _telemetry.TrackPageView($"Sign-in:Kiosk");

            return Redirect("https://kiosk.wggdemo.net");
        }
    }
}
