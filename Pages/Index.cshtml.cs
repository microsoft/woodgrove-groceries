using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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
        // Application insights telemetry
        PageViewTelemetry pageView = new PageViewTelemetry("Home page");

        if (Request.Headers.ContainsKey("Referer"))
        {
            // Check the referer of the request
            string referer = Request.Headers["Referer"].ToString();
            try
            {
                // Get the host name
                var uri = new System.Uri(referer);
                pageView.Properties.Add("Referral", uri.Host.ToLower());

                // Add the full URL
                pageView.Properties.Add("ReferralURL", referer);
            }
            catch (System.Exception ex)
            {
                pageView.Properties.Add("Referral", "Invalid");
                pageView.Properties.Add("ReferralURL", referer);
            }
        }
        else
        {
            pageView.Properties.Add("Referral", "Unknown");
        }
        
        _telemetry.TrackPageView(pageView);

        this.listOfAvatars = new List<string>() { "01", "02", "03", "04", "05", "06", "07" };

        // Randomly Order it by Guid..
        this.listOfAvatars = this.listOfAvatars.OrderBy(i => Guid.NewGuid()).ToList();

        if (User.Identity?.IsAuthenticated == true)
        {
            // Check whether the account is commercial
            string commercialAccountsSecurityGroup = Configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value;
            IsCommercialAccount = User.Claims.Any(c => c.Type == "groups" && c.Value == commercialAccountsSecurityGroup);

            // Check the Eggs allergy
            string? specialDiet = User.Claims.FirstOrDefault(c => c.Type.ToLower() == "specialdiet")?.Value;
            HasEggsAllergy = (string.IsNullOrEmpty(specialDiet) == false && specialDiet.ToLower().StartsWith("egg"));
        }

        return Page();
    }
}
