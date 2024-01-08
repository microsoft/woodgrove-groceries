using System.Dynamic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Azure.Identity;
using Microsoft.ApplicationInsights;
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
    public void RecordUseCase(string ID)
    {
        string[] useCases = { "Default", "OnlineRetail", "EmailAndPassword", "SSO", "ForceSignIn", "UserInsights", "BlockSignUp", "CompanyBranding", "Language", "SSPR", "Social", "TokenAugmentation", "PreAttributeCollection", "PostAttributeCollection", "ProfileEdit", "DeleteAccount", "RBAC", "GBAC", "CustomAttributes", "Kiosk", "Finance" };

        if (useCases.Contains(ID))
        {
            _telemetry.TrackEvent(ID); ;
        }
        else
        {
            _telemetry.TrackEvent("Unknown"); ;
        }
    }
}
