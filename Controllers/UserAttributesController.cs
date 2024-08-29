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
[Route("[controller]")]
public class UserAttributesController : ControllerBase
{

    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;
    private readonly GraphServiceClient _graphServiceClient;
    readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;
    private string ExtensionAttributes { get; set; } = "";

    public UserAttributesController(IConfiguration configuration, TelemetryClient telemetry, GraphServiceClient graphServiceClient, IAuthorizationHeaderProvider authorizationHeaderProvider)
    {
        _configuration = configuration;
        _telemetry = telemetry;

        // Get the app settings
        ExtensionAttributes = _configuration.GetSection("MicrosoftGraph:ExtensionAttributes").Value!;
        _graphServiceClient = graphServiceClient;
        _authorizationHeaderProvider = authorizationHeaderProvider;
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
            AppInsights.TrackException(_telemetry, odataError, "ReadProfile");
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            att.ErrorMessage = $"Can't read the profile due to the following error: {error}";
            AppInsights.TrackException(_telemetry, ex, "ReadProfile");
        }

        return Ok(att);
    }

    [HttpPost]
    public async Task<IActionResult> OnPostAsync([FromForm] UserAttributes att)
    {

        _telemetry.TrackPageView("Profile:Update");

        // Read app settings
        string baseUrl = _configuration.GetSection("GraphApiMiddleware:BaseUrl").Value!;
        string[] scopes = _configuration.GetSection("GraphApiMiddleware:Scopes").Get<string[]>();
        string endpoint = _configuration.GetSection("GraphApiMiddleware:Endpoint").Value!;

        // Check the scopes application settings
        if (scopes == null)
        {
            att.ErrorMessage = "The GraphApiMiddleware:Scopes application setting is misconfigured or missing. Use the array format: [\"Account.Payment\", \"Account.Purchases\"]";
            return Ok();
        }

        // Check the base URL application settings
        if (string.IsNullOrEmpty(baseUrl))
        {
            att.ErrorMessage = "The GraphApiMiddleware:BaseUrl application setting is misconfigured or missing. Check out your applications' scope base URL in Microsoft Entra admin center. For example: api://12345678-0000-0000-0000-000000000000";
            return Ok();
        }

        // Check the endpoint application settings
        if (string.IsNullOrEmpty(endpoint))
        {
            att.ErrorMessage = "The GraphApiMiddleware:Endpoint application setting is misconfigured or missing.";
            return Ok();
        }

        // Set the scope full URL (temporary workaround should be fix)
        for (int i = 0; i < scopes.Length; i++)
        {
            scopes[i] = $"{baseUrl}/{scopes[i]}";
        }

        try
        {
            // Get an access token to call the Graph middleware API
            string accessToken = await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(scopes);

            // Use the access token to call the Graph middleware API.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", accessToken);
            var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("ObjectId", att.ObjectId),
                    new KeyValuePair<string, string>("City", att.City),
                    new KeyValuePair<string, string>("Country", att.Country),
                    new KeyValuePair<string, string>("DisplayName", att.DisplayName),
                    new KeyValuePair<string, string>("GivenName", att.GivenName),
                    new KeyValuePair<string, string>("SpecialDiet", att.SpecialDiet),
                    new KeyValuePair<string, string>("Surname", att.Surname)
                });

            var httpResponseMessage = await client.PostAsync(endpoint, formContent);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            return Ok(responseContent);
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            att.ErrorMessage = $"The account cannot be updated due to the following error: {error}";
            AppInsights.TrackException(_telemetry, ex, "OnPostProfileAsync");
        }

        return Ok(att);
    }

}
