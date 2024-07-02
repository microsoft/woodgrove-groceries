using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    [Authorize]
    //[Authorize(Roles = "Products.Contributor")]
    public class ProductsModel : PageModel
    {
        public bool UserHasAccess { get; private set; } = false;
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public ProductsModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("Products");

            // Check if user is assigned to the Products.Contributor application role
            UserHasAccess = User.IsInRole("Products.Contributor");

            return Page();
        }
    }
}
