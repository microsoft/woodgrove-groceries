using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace MyApp.Namespace
{

    [AllowAnonymous]
    public class SignInModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private TelemetryClient _telemetry;
        readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

        public SignInModel(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            _configuration = configuration;
            _telemetry = telemetry;
            _authorizationHeaderProvider = authorizationHeaderProvider;
        }

        private IActionResult TrackAndAuth(string ID,
            string redirectUri = "/",
            bool reauth = false, string?
            extraParam = null, string?
            extraParamValue = null,
            string? ui_locales = null,
            string? login_hint = null)
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
            if (ID == "StepUp" || ID == "EditProfileMfa" || ID == "MFA")
            {
                challengeResult.Properties.Items.Add("StepUp", "true");
            }


            // ui_locales
            if (ID == "PreSelectLanguage")
            {
                challengeResult.Properties.Items.Add("ui_locales", ui_locales);
            }

            // Propmt create
            if (ID == "SignUpLink")
            {
                challengeResult.Properties.Items.Add("prompt", "create");
            }

            // login_hint
            if (!string.IsNullOrEmpty(login_hint))
            {
                challengeResult.Properties.Items.Add("login_hint", login_hint);
            }

            // Extra parameter
            if (!string.IsNullOrEmpty(extraParam) && !string.IsNullOrEmpty(extraParamValue))
            {
                challengeResult.Properties.Items.Add(extraParam, extraParamValue);
            }

            return challengeResult;
        }


        public IActionResult OnGetDefault()
        {
            return this.TrackAndAuth("Default");
        }

        public IActionResult OnGetModifyAttributeValues()
        {
            return this.TrackAndAuth("ModifyAttributeValues", "/", true);
        }

        public IActionResult OnGetBlockSignUp()
        {
            return this.TrackAndAuth("BlockSignUp", "/", true);
        }
        public IActionResult OnGetCSA()
        {

            _telemetry.TrackPageView($"Sign-in:CSA");

            return Redirect(this._configuration.GetSection("Demos:CustomSecurityAttributesURL").Value);
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

        public IActionResult OnGetEditProfileMfa()
        {
            return this.TrackAndAuth("EditProfileMfa", "/profile", false);
        }

        public IActionResult OnGetUserLastActivity()
        {
            return this.TrackAndAuth("UserLastActivity", "/profile", true);
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
        public IActionResult OnGetPreSelectLanguage()
        {
            return this.TrackAndAuth("PreSelectLanguage", "/", true, null, null, this.Request.Query["ui_locales"]);
        }
        public IActionResult OnGetProfileSignin(string? id)
        {
            return this.TrackAndAuth("ProfileSignin", "/", false, null, null, null, id);
        }

        public IActionResult OnGetLoginHint(string id)
        {
            return this.TrackAndAuth("LoginHint", "/", true, null, null, null, id);
        }

        public async Task<IActionResult> OnGetActAs(string id)
        {
            await StartActAsAsync(id);
            return this.TrackAndAuth("ActAs", "/Token", false);
        }

        public IActionResult OnGetSignUpLink(string id)
        {
            return this.TrackAndAuth("SignUpLink", "/", false, null, null, null, id);
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

        public IActionResult OnGetTokenTTL()
        {
            return this.TrackAndAuth("TokenTTL", "/", true);
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

        public IActionResult OnGetDisableAccount()
        {
            return this.TrackAndAuth("DisableAccount", "/Profile", true);
        }

        public IActionResult OnGetSSO2()
        {

            _telemetry.TrackPageView($"Sign-in:SSO-Continue");

            return Redirect(this._configuration.GetSection("Demos:WoodgroveBankURL").Value + "/Auth/Login");
        }

        public IActionResult OnGetAssignmentRequired()
        {
            _telemetry.TrackPageView($"Sign-in:AssignmentRequired");

            return Redirect(this._configuration.GetSection("Demos:AssignmentRequiredURL").Value);
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

        public IActionResult OnGetCustomDomain()
        {
            return this.TrackAndAuth("CustomDomain", "/", true, "domain", this._configuration.GetSection("Demos:CustomDomain").Value);
        }

        public IActionResult OnGetCustomEmail()
        {
            return this.TrackAndAuth("CustomEmail", "/", true);
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


        private async Task StartActAsAsync(string id)
        {
            // Input validation 
            if (id.Length > 20)
            {
                id = id.Substring(0, 20);
            }

            // Read app settings
            string baseUrl = _configuration.GetSection("WoodgroveGroceriesAuthApi:BaseUrl").Value!;
            string[] scopes = _configuration.GetSection("WoodgroveGroceriesAuthApi:Scopes").Get<string[]>();
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
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                // TBD
            }
            catch (System.Exception ex)
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
            catch (System.Exception ex)
            {
                // TBD
            }
        }
    }
}
