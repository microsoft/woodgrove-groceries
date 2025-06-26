using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages;

public class IndexModel : PageModel
{
    public bool IsCommercialAccount { get; set; } = false;
    public bool HasEggsAllergy { get; set; } = false;
    public bool StepUpFulfilled { get; set; } = false;
    public string Alert { get; set; } = string.Empty;
    public string LoyaltyNumber { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = new List<Product>();
    public List<string> Categories { get; set; } = new List<string>();
    public Random random = new Random();

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

       // Get the products
        Products = ProductData.GetSampleProducts()
            .OrderBy(x => random.Next())
            .ToList();

        // Get unique categories for dropdown
        Categories = Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        if (User.Identity?.IsAuthenticated == true)
        {
            // Check whether the account is commercial
            string commercialAccountsSecurityGroup = Configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value;
            IsCommercialAccount = User.Claims.Any(c => c.Type == "groups" && c.Value == commercialAccountsSecurityGroup);

            // Check the Eggs allergy
            string? specialDiet = User.Claims.FirstOrDefault(c => c.Type.ToLower() == "specialdiet")?.Value;
            HasEggsAllergy = (string.IsNullOrEmpty(specialDiet) == false && specialDiet.ToLower().StartsWith("egg"));

            // Check if the step up completed
            StepUpFulfilled = User.Claims.Any(c => c.Type == "acrs" && c.Value == "c1");

            // Read the Loyalty number claim
            LoyaltyNumber = User.Claims.FirstOrDefault(c => c.Type.ToLower() == "loyaltynumber")?.Value;
        }



        // Read the system alert
        Alert = Configuration.GetSection("Demos:Alert").Value;

        return Page();
    }
}
