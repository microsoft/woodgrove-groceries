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
[Route("api/[controller]")]
public class UserMoreInfoController : ControllerBase
{
    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;

    public UserMoreInfoController(IConfiguration configuration, TelemetryClient telemetry)
    {
        _configuration = configuration;
        _telemetry = telemetry;
    }

    public async Task<IActionResult> GetAsync()
    {
        UserMoreInfo userMoreInfo = new UserMoreInfo();

        // Get the user unique identifier
        string? userObjectId = User.GetObjectId();

        var graphClient = MsalAccessTokenHandler.GetGraphClient(_configuration);

        try
        {
            User profile = await graphClient.Users[userObjectId].GetAsync(requestConfiguration =>
                                {
                                    requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities", "signInActivity" };
                                });


            // Get the sign-in activity
            if (profile.SignInActivity != null)
            {
                userMoreInfo.LastSignInDateTime = profile!.SignInActivity!.LastSignInDateTime!.ToString()!;
                userMoreInfo.LastSignInRequestId = profile!.SignInActivity.LastSignInRequestId!;
            }
            else
            {
                userMoreInfo.LastSignInDateTime = "Data is not yet available.";
                userMoreInfo.LastSignInRequestId = userMoreInfo.LastSignInDateTime;
            }


            // Get the user identities
            foreach (var identity in profile!.Identities!)
            {
                if (identity.SignInType == "userPrincipalName")
                {
                    continue;
                }
                else
                {
                    if (identity.SignInType == "emailAddress")
                    { 
                        // Get the sign-in email address
                        userMoreInfo.SingInEmail = identity.IssuerAssignedId!;
                    }

                    userMoreInfo.Identities += $"<li><b>Sign-in type:</b> {identity.SignInType} <b>Issuer</b>: {identity.Issuer} <b>ID</b>: {identity.IssuerAssignedId} <ui/>";
                }

                // Get the username of local accounts
                // if (identity.SignInType == "emailAddress")
                // {
                //     this.Username = identity.IssuerAssignedId!;
                // }
            }
        }
        catch (ODataError odataError)
        {
            userMoreInfo.ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "GetRolesAndGroupsAsync");
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            userMoreInfo.ErrorMessage = $"Can't read the profile due to the following error: {error}";
            AppInsights.TrackException(_telemetry, ex, "GetRolesAndGroupsAsync");
        }

        bool StepUpFulfilled = User.Claims.Any(c => c.Type == "acrs" && c.Value == "c1");
        try
        {
            var result = await graphClient.Users[userObjectId].Authentication.Methods.GetAsync();

            foreach (var method in result.Value)
            {
                if (method.OdataType == "#microsoft.graph.phoneAuthenticationMethod")
                {
                    userMoreInfo.PhoneNumber = ((PhoneAuthenticationMethod)method).PhoneNumber;
                }
                else if (method.OdataType == "#microsoft.graph.emailAuthenticationMethod")
                {
                    userMoreInfo.EmailMfa = ((EmailAuthenticationMethod)method).EmailAddress;
                }
            }

            if (string.IsNullOrEmpty(userMoreInfo.PhoneNumber))
                userMoreInfo.PhoneNumber = "";

            if (string.IsNullOrEmpty(userMoreInfo.EmailMfa))
                userMoreInfo.EmailMfa = "";
        }
        catch (ODataError odataError)
        {
            userMoreInfo.ErrorMessage = $"Can't read the authentication methods due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "GetAuthenticationMethodsAsync");
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            userMoreInfo.ErrorMessage = $"Can't read the authentication methods due to the following error: {error}";
            AppInsights.TrackException(_telemetry, ex, "GetAuthenticationMethodsAsync");
        }

        return Ok(userMoreInfo);
    }
}
