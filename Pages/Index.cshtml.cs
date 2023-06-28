using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages;

public class IndexModel : PageModel
{
    public bool IsCommercialAccount { get; set; } = false;
    public bool HasEggsAllergy { get; set; } = false;

    public List<string> listOfAvatars { get; set; }

    private readonly ILogger<IndexModel> _logger;
    private TelemetryClient _telemetry;
    private readonly IConfiguration Configuration;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, TelemetryClient telemetry)
    {
        _logger = logger;
        _telemetry = telemetry;
        Configuration = configuration;
    }

    public IActionResult OnGet()
    {
        _telemetry.TrackPageView("Home page");

        this.listOfAvatars = new List<string>() { "01", "02", "03", "04", "05", "06", "07" };

        // Randomly Order it by Guid..
        this.listOfAvatars = this.listOfAvatars.OrderBy(i => Guid.NewGuid()).ToList();

        if (User.Identity?.IsAuthenticated == true)
        {
            // Check whether the account is commercial
            string commercialAccountsSecurityGroup = Configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value;
            IsCommercialAccount = User.Claims.Any(c => c.Type == "groups" && c.Value == commercialAccountsSecurityGroup);

            // Check the Eggs allergy
            string? specialDiet = User.Claims.FirstOrDefault(c => c.Type == "SpecialDiet")?.Value;
            HasEggsAllergy = (string.IsNullOrEmpty(specialDiet) == false && specialDiet.ToLower().StartsWith("egg"));
        }

        return Page();
    }
}
