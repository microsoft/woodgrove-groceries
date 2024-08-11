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
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public ProfileModel(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _telemetry.TrackPageView("Profile:Read");

            return Page();
        }
    }
}

