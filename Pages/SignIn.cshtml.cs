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

        public IActionResult OnGetBlockSignUp()
        {
            return this.TrackAndAuth("BlockSignUp");
        }

        public IActionResult OnGetOnlineRetail()
        {
            return this.TrackAndAuth("OnlineRetail");
        }

        public IActionResult OnGetCompanyBranding()
        {
            return this.TrackAndAuth("CompanyBranding");
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

        public IActionResult OnGetSSO1()
        {
            return this.TrackAndAuth("SSO-Start");
        }

        public IActionResult OnGetSSO2()
        {

            _telemetry.TrackPageView($"Sign-in:SSO-Continue");

            return Redirect("https://woodgrovebanking.com");
        }

        public IActionResult OnGetTokenAugmentation()
        {
            return this.TrackAndAuth("TokenAugmentation");
        }

        public IActionResult OnGetPreAttributeCollection()
        {
            return this.TrackAndAuth("PreAttributeCollection");
        }

        public IActionResult OnGetPostAttributeCollection()
        {
            return this.TrackAndAuth("PostAttributeCollection");
        }

        public IActionResult OnGetRBAC()
        {
            return this.TrackAndAuth("RBAC");
        }

        public IActionResult OnGetGBAC()
        {
            return this.TrackAndAuth("GBAC");
        }

        public IActionResult OnGetSSPR()
        {
            return this.TrackAndAuth("SSPR");
        }

        public IActionResult OnGetCustomAttributes()
        {
            return this.TrackAndAuth("CustomAttributes");
        }

        public IActionResult OnGetKiosk()
        {
            _telemetry.TrackPageView($"Sign-in:Kiosk");

            return Redirect("https://woodgroverestaurants.com");
        }

        public IActionResult OnGetFinance()
        {
            _telemetry.TrackPageView($"Sign-in:Kiosk");

            return Redirect("https://woodgrovebanking.com/");
        }
    }
}
