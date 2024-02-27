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

        private IActionResult TrackAndAuth(string ID, string redirectUri = "/", bool reauth = false)
        {
            _telemetry.TrackPageView($"Sign-in:{ID}");

            ChallengeResult challengeResult = new ChallengeResult(
                OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                });

            // Force re-authentication
            if (this.Request.Query["force"].Count > 0 || reauth)
            {
                challengeResult.Properties.Items.Add("force", "true");
            }

            // Step up flow
            if (ID == "StepUp" || ID == "MFA")
            {
                challengeResult.Properties.Items.Add("StepUp", "true");
            }

            return challengeResult;
        }


        public IActionResult OnGetDefault()
        {
            return this.TrackAndAuth("Default");
        }

        public IActionResult OnGetBlockSignUp()
        {
            return this.TrackAndAuth("BlockSignUp", "/", true);
        }

        public IActionResult OnGetCSA()
        {

            _telemetry.TrackPageView($"Sign-in:CSA");

            return Redirect(this.Configuration.GetSection("Demos:CustomSecurityAttributesURL").Value);
        }

        public IActionResult OnGetStepUp()
        {
            return this.TrackAndAuth("StepUp", "/#cmd=StepUpCompleted", false);
        }

        public IActionResult OnGetStepUpIntro()
        {
            return this.TrackAndAuth("StepUp-Start", "/#usecase=StepUp", true);
        }

        public IActionResult OnGetPolicyAgreement()
        {
            return this.TrackAndAuth("PolicyAgreement", "/", true);
        }

        public IActionResult OnGetMfa()
        {
            return this.TrackAndAuth("MFA", "/", true);
        }

        public IActionResult OnGetActivity()
        {
            return this.TrackAndAuth("Activity", "/profile", true);
        }

        public IActionResult OnGetCa()
        {
            return this.TrackAndAuth("CA", "/", true);
        }

        public IActionResult OnGetObo()
        {
            return this.TrackAndAuth("OBO", "/token", true);
        }

        public IActionResult OnGetOnlineRetail()
        {
            return this.TrackAndAuth("OnlineRetail", "/", true);
        }

        public IActionResult OnGetCompanyBranding()
        {
            return this.TrackAndAuth("CompanyBranding", "/", true);
        }

        public IActionResult OnGetLanguage()
        {
            return this.TrackAndAuth("Language", "/", true);
        }

        public IActionResult OnGetProfileSignin()
        {
            return this.TrackAndAuth("ProfileSignin", "/", true);
        }

        public IActionResult OnGetTokenSignin()
        {
            return this.TrackAndAuth("TokenSignin", "/token");
        }

        public IActionResult OnGetEmailAndPassword()
        {
            return this.TrackAndAuth("EmailAndPassword", "/", true);
        }

        public IActionResult OnGetSocial()
        {
            return this.TrackAndAuth("Social", "/", true);
        }

        public IActionResult OnGetSSO1()
        {
            return this.TrackAndAuth("SSO-Start");
        }

        public IActionResult OnGetForceSignIn()
        {
            return this.TrackAndAuth("ForceSignIn", "/", true);
        }

        public IActionResult OnGetInvalidSession()
        {
            return this.TrackAndAuth("ForceSignIn", "/", true);
        }

        public IActionResult OnGetSSO2()
        {

            _telemetry.TrackPageView($"Sign-in:SSO-Continue");

            return Redirect("https://bank.woodgrovedemo.com/Auth/Login");
        }

        public IActionResult OnGetTokenAugmentation()
        {
            return this.TrackAndAuth("TokenAugmentation", "/", true);
        }

        public IActionResult OnGetTokenClaims()
        {
            return this.TrackAndAuth("TokenClaims", "/", true);
        }

        public IActionResult OnGetPreAttributeCollection()
        {
            return this.TrackAndAuth("PreAttributeCollection", "/", true);
        }

        public IActionResult OnGetPostAttributeCollection()
        {
            return this.TrackAndAuth("PostAttributeCollection", "/", true);
        }

        public IActionResult OnGetRBAC()
        {
            return this.TrackAndAuth("RBAC", "/", true);
        }

        public IActionResult OnGetGBAC()
        {
            return this.TrackAndAuth("GBAC", "/", true);
        }

        public IActionResult OnGetSSPR()
        {
            return this.TrackAndAuth("SSPR", "/", true);
        }

        public IActionResult OnGetCustomAttributes()
        {
            return this.TrackAndAuth("CustomAttributes", "/", true);
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
