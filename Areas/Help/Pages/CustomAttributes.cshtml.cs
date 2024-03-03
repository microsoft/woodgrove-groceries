using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Help.Pages
{
    public class CustomAttributesModel : PageModel
    {
        private TelemetryClient _telemetry;
        public CustomAttributesModel(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        public void OnGet()
        {
            PageViewTelemetry pageView = new PageViewTelemetry("CustomAttributes");

            // Type of the page
            pageView.Properties.Add("Area", "Help");
            _telemetry.TrackPageView(pageView);
        }
    }
}
