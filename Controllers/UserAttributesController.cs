using System.Dynamic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
[Route("[controller]")]
public class UserAttributesController : ControllerBase
{

    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;
    private readonly GraphServiceClient _graphServiceClient;
    private string ExtensionAttributes { get; set; } = "";

    public UserAttributesController(IConfiguration configuration, TelemetryClient telemetry, GraphServiceClient graphServiceClient)
    {
        _configuration = configuration;
        _telemetry = telemetry;

        // Get the app settings
        ExtensionAttributes = _configuration.GetSection("MicrosoftGraph:ExtensionAttributes").Value!;
        _graphServiceClient = graphServiceClient; ;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        UserAttributes att = new UserAttributes();

        try
        {
            User profile = await _graphServiceClient.Me.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities", "displayName", "GivenName", "Surname", "Country", "City", "AccountEnabled", "CreatedDateTime", "lastPasswordChangeDateTime", $"{ExtensionAttributes}_SpecialDiet" };
                    requestConfiguration.QueryParameters.Expand = new string[] { "Extensions" };
                });


            // Populate the user's attributes
            att.ObjectId = profile!.Id!;
            att.DisplayName = profile!.DisplayName ?? "";
            att.Surname = profile!.Surname ?? "";
            att.GivenName = profile!.GivenName ?? "";
            att.Country = profile!.Country ?? "";
            att.City = profile!.City ?? "";

            if (profile!.AccountEnabled != null)
                att.AccountEnabled = (bool)profile!.AccountEnabled;

            // Get the special diet from the extension attributes
            object specialDiet;
            if (profile.AdditionalData.TryGetValue($"{ExtensionAttributes}_SpecialDiet", out specialDiet))
            {
                att.SpecialDiet = specialDiet.ToString();
            }


            // Get the account creation time
            if (profile.CreatedDateTime != null)
            {
                att.CreatedDateTime = profile.CreatedDateTime.ToString();
            }

            // Get the last time user changed their password
            if (profile.LastPasswordChangeDateTime != null)
            {
                att.LastPasswordChangeDateTime = profile.LastPasswordChangeDateTime.ToString();
            }
            else
            {
                att.LastPasswordChangeDateTime = "Data is not available. It might be because you sign-in with a federated account, or email and one time passcode.";
            }

            return Ok(att);
        }
        catch (ODataError odataError)
        {
            att.ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            //TrackException(odataError, "ReadProfile");
        }
        catch (Exception ex)
        {
            att.ErrorMessage = $"Can't read the profile due to the following error: {ex.Message}";
            //TrackException(ex, "ReadProfile");
        }

        return Ok(att);
    }

    [HttpPost]
    public async Task<IActionResult> OnPostAsync([FromForm] UserAttributes att)
    {

        _telemetry.TrackPageView("Profile:Update");

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
                DisplayName = att.DisplayName,
                GivenName = att.GivenName,
                Surname = att.Surname,
                Country = att.Country,
                City = att.City,
                AdditionalData = new Dictionary<string, object>
                    {
                        {
                            $"{ExtensionAttributes}_SpecialDiet" , att.SpecialDiet
                        },
                    },
                AccountEnabled = att.AccountEnabled
            };

            var graphClient = MsalAccessTokenHandler.GetGraphClient(_configuration);
            var result = await graphClient.Users[userObjectId].PatchAsync(requestBody);
            //var result = await _graphServiceClient.Me.PatchAsync(requestBody);
        }
        catch (ODataError odataError)
        {
            att.ErrorMessage = $"The account cannot be updated due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            //TrackException(odataError, "OnPostProfileAsync");
        }
        catch (Exception ex)
        {
            att.ErrorMessage = $"The account cannot be updated due to the following error: {ex.Message}";
            //TrackException(ex, "OnPostProfileAsync");
        }

        return Ok(att);
    }

}
