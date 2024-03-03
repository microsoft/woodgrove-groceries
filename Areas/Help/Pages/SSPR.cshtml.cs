using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Help.Pages
{
    public class SSPRModel : PageModel
    {
        private TelemetryClient _telemetry;
        public SSPRModel(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        public void OnGet()
        {
            PageViewTelemetry pageView = new PageViewTelemetry("SSPR");

            // Type of the page
            pageView.Properties.Add("Area", "Help");
            _telemetry.TrackPageView(pageView);
        }
    }
}
