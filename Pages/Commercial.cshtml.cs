using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    [Authorize(Policy = "CommercialOnly")]
    public class CommercialModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public CommercialModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }
        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("Commercial");

            return Page();
        }
    }
}
