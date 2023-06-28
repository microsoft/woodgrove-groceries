using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
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
