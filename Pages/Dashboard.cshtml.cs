using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public DashboardModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }
        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("Dashboard");

            return Page();
        }
    }
}
