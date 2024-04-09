using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Help.Pages
{
    public class TokenLifeTimeModel : PageModel
    {
        private TelemetryClient _telemetry;
        public TokenLifeTimeModel(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        public void OnGet()
        {
            PageViewTelemetry pageView = new PageViewTelemetry("TokenLifeTime");

            // Type of the page
            pageView.Properties.Add("Area", "Help");
            _telemetry.TrackPageView(pageView);
        }
    }
}
