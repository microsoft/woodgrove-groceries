using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    [Authorize(Policy = "ExclusiveDemosOnly")]
    public class ChatModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public ChatModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }
        public IActionResult OnGet()
        {
            // Set the page id for telemetry
            // This is used to track the page view in Application Insights
            _telemetry.TrackPageView("Chat");

            return Page();
        }
    }
}
