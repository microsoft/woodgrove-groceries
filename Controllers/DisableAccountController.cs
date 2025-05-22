using System.Dynamic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using woodgrovedemo.Helpers;
using woodgrovedemo.Models;

namespace woodgrovedemo.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DisableAccountController : ControllerBase
{

    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;
    private readonly GraphServiceClient _graphServiceClient;

    public DisableAccountController(IConfiguration configuration, TelemetryClient telemetry, GraphServiceClient graphServiceClient)
    {
        _configuration = configuration;
        _telemetry = telemetry;
        _graphServiceClient = graphServiceClient;
    }


    [HttpGet]
    public async Task<IActionResult> OnGetAsync()
    {

        _telemetry.TrackPageView("Profile:Disable");
        UserAttributes att = new UserAttributes();

        // Get the user unique identifier
        string? userObjectId = User.GetObjectId();

        if (userObjectId == null)
        {
            att.ErrorMessage = "The account cannot be updated since your access token doesn't contain the required 'objectidentifier' claim.";
        }

        try
        {
            // Update user by object ID
            var requestBody = new User
            {
                AccountEnabled = false
            };

            var graphClient = MsalAccessTokenHandler.GetGraphClient(_configuration);
            var result = await graphClient.Users[userObjectId].PatchAsync(requestBody);
        }
        catch (ODataError odataError)
        {
            att.ErrorMessage = $"The account cannot be updated due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "OnPostProfileAsync");
        }
        catch (Exception ex)
        {
            att.ErrorMessage = $"The account cannot be updated due to the following error: {ex.Message}";
            AppInsights.TrackException(_telemetry, ex, "OnPostProfileAsync");
        }

        return Ok();
    }
}
