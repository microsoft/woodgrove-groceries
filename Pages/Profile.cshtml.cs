using System.ComponentModel;
using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using woodgrovedemo.Helpers;

namespace MyApp.Namespace
{

    [Authorize]
    public class ProfileModel : PageModel
    {
        // Dependency injection
        private TelemetryClient _telemetry;

        // Local members
        public bool StepUpFulfilled { get; set; } = false;
        
        public ProfileModel(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("Profile:Read");
            StepUpFulfilled = User.Claims.Any(c => c.Type == "acrs" && c.Value == "c1");

            return Page();
        }
    }
}

