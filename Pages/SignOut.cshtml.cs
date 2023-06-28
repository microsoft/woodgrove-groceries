using Microsoft.ApplicationInsights;
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

            return Redirect("/MicrosoftIdentity/Account/SignOut");
        }
    }
}
