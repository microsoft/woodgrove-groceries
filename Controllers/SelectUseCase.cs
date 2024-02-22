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
        string[] useCases = { "Default", "OnlineRetail", "StepUp", "CSA", "PolicyAgreement", "EmailAndPassword", "OBO", "SSO", "MFA", "CA", "ForceSignIn", "UserInsights", "BlockSignUp", "CompanyBranding", "Language", "SSPR", "Social", "TokenAugmentation", "TokenClaims", "PreAttributeCollection", "PostAttributeCollection", "ProfileEdit", "DeleteAccount", "Activity", "RBAC", "GBAC", "CustomAttributes", "Kiosk", "Finance" };

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
            // Known use case ID
            EventTelemetry eventTelemetry = new EventTelemetry(ID);
            eventTelemetry.Properties.Add("Referral", referralDomain);
            eventTelemetry.Properties.Add("ReferralURL", referral);
            _telemetry.TrackEvent(eventTelemetry); ;
        }
        else
        {
            //Unknown use case ID
            EventTelemetry eventTelemetry = new EventTelemetry("Unknown");
            eventTelemetry.Properties.Add("Referral", referralDomain);
            eventTelemetry.Properties.Add("ReferralURL", referral);

            // Check the length of the unsupported use case
            if (ID.Length > 26)
            {
                eventTelemetry.Properties.Add("UnknownValue", ID.Substring(0, 25));
            }
            else
            {
                eventTelemetry.Properties.Add("UnknownValue", ID);
            }

            _telemetry.TrackEvent(eventTelemetry); ;
        }
    }
}
