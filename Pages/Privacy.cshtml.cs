using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages;

[AllowAnonymous]
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    private readonly IConfiguration Configuration;
    private TelemetryClient _telemetry;

    public PrivacyModel(ILogger<PrivacyModel> logger, IConfiguration configuration, TelemetryClient telemetry)
    {
        _logger = logger;
        Configuration = configuration;
        _telemetry = telemetry;
    }

    public IActionResult OnGet()
    {
        _telemetry.TrackPageView("Privacy");

        return Page();
    }
}

