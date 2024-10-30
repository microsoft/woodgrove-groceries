using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    [AllowAnonymous]
    public class TOSModel : PageModel
    {

        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public TOSModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("TOS");
            
            return Page();
        }
    }
}
