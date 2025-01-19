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
    public IActionResult RecordUseCase(string ID, string trigger, string referral = null)
    {
        string[] useCases = { "Default", "SignUpLink", "CustomDomain", "AssignmentRequired", "OnlineRetail", "StepUp", "CSA", "PolicyAgreement", "EmailAndPassword", "DisableAccount", "OBO", "SSO", "TokenTTL", "MFA", "CA", "ForceSignIn", "UserInsights", "ModifyAttributeValues", "BlockSignUp", "CompanyBranding", "Language", "PreSelectLanguage", "SSPR", "Social", "ActAs", "LoginHint", "TokenAugmentation", "GithubWorkflows", "TokenClaims", "PreAttributeCollection", "PostAttributeCollection", "ProfileEdit", "DeleteAccount", "UserLastActivity", "RBAC", "GBAC", "CustomAttributes", "Kiosk", "Finance" };
        string[] triggers = { "Link", "Start", "Select" };

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

        // Check the trigger ID
        if (!triggers.Contains(trigger))
        {
            trigger = "Unknown";
        }


        // Check the event ID
        string eventIDToRecord = ID;
        string? UnknownValue = null;
        if (!useCases.Contains(eventIDToRecord))
        {
            eventIDToRecord = "Unknown";

            // Check the length of the unsupported use case
            if (ID.Length > 26)
            {
                UnknownValue = ID.Substring(0, 25);
            }
            else
            {
                UnknownValue = ID;
            }
        }

        // Create telemetry event
        EventTelemetry eventTelemetry = new EventTelemetry(eventIDToRecord);
        eventTelemetry.Properties.Add("Referral", referralDomain);
        eventTelemetry.Properties.Add("ReferralURL", referral);
        eventTelemetry.Properties.Add("Trigger", trigger);
        eventTelemetry.Properties.Add("Event", "ShowUseCase");

        // Check for unknown value
        if (UnknownValue != null)
        {
            eventTelemetry.Properties.Add("UnknownValue", UnknownValue);
        }

        _telemetry.TrackEvent(eventTelemetry); ;

        return Ok();
    }
}
