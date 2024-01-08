using System.Dynamic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("[controller]")]
public class SelectUseCaseController : ControllerBase
{

    private TelemetryClient _telemetry;


    public SelectUseCaseController(TelemetryClient telemetry)
    {
        _telemetry = telemetry;
    }

    [HttpGet("usecase")]
    public void RecordUseCase(string ID, string referral = null)
    {
        string[] useCases = { "Default", "OnlineRetail", "EmailAndPassword", "SSO", "ForceSignIn", "UserInsights", "BlockSignUp", "CompanyBranding", "Language", "SSPR", "Social", "TokenAugmentation", "PreAttributeCollection", "PostAttributeCollection", "ProfileEdit", "DeleteAccount", "RBAC", "GBAC", "CustomAttributes", "Kiosk", "Finance" };
        string referralDomain = string.Empty;

        // Check if the referral is available
        if (!string.IsNullOrEmpty(referral))
        {
            try
            {
                // Get the host name
                var uri = new System.Uri(referral);
                referralDomain = uri.Host.ToLower();
            }
            catch (System.Exception ex)
            {
                referralDomain = "Invalid";
            }
        }
        else
        {
            // if the referral is not available set to unknown
            referralDomain = "Unknown";
        }

        if (useCases.Contains(ID))
        {
            EventTelemetry eventTelemetry = new EventTelemetry(ID);
            eventTelemetry.Properties.Add("Referral", referralDomain);
            eventTelemetry.Properties.Add("ReferralURL", referral);
            _telemetry.TrackEvent(eventTelemetry); ;
        }
        else
        {
            EventTelemetry eventTelemetry = new EventTelemetry("Unknown");
            eventTelemetry.Properties.Add("Referral", referralDomain);
            eventTelemetry.Properties.Add("ReferralURL", referral);

            if (ID.Length > 26)
            {
                eventTelemetry.Properties.Add("UnknownValue", referral);
            }
            else
            {
                eventTelemetry.Properties.Add("UnknownValue", referral.Substring(0, 25));
            }

            _telemetry.TrackEvent(eventTelemetry); ;
        }
    }
}
