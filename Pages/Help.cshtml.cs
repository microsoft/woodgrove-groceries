using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    [AllowAnonymous]
    public class HelpModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public HelpModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }
        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("Help");

            return Page();
        }
    }
}
