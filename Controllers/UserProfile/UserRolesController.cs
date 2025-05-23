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
public class UserRolesController : ControllerBase
{
    // Dependency injection
    private readonly IConfiguration _configuration;
    private TelemetryClient _telemetry;

    public UserRolesController(IConfiguration configuration, TelemetryClient telemetry)
    {
        _configuration = configuration;
        _telemetry = telemetry;
    }

    private async Task<UserRoles> GetUserRolesAndGroups()
    {
        UserRoles userRoles = new UserRoles();

        // Get the user unique identifier
        string? userObjectId = User.GetObjectId();

        var graphClient = MsalAccessTokenHandler.GetGraphClient(_configuration);

        // Get the user's security groups
        var groups = await graphClient.Users[userObjectId].MemberOf.GetAsync();

        // Check if the user is a member of the commercial accounts security proup
        if (groups?.Value != null)
        {
            foreach (var group in groups.Value)
            {

                if (((Group)group).Id == _configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value)
                {
                    userRoles.MemberOfCommercialAccounts = true;
                    break;
                }
            }
        }

        // Get the user's app role assignments
        var appRoleAssignments = await graphClient.Users[userObjectId].AppRoleAssignments.GetAsync();

        // Check if the user has any app role assignments
        if (appRoleAssignments != null && appRoleAssignments.Value != null)
        {
            foreach (var appRoleAssignment in appRoleAssignments.Value)
            {

                // Check the Products.Contributor role
                if (((AppRoleAssignment)appRoleAssignment).AppRoleId.ToString() == _configuration.GetSection("AppRoles:ProductsContributor").Value)
                {
                    userRoles.HasProductsContributorRole = true;
                    userRoles.ProductsContributorAssignmentId = appRoleAssignment.Id != null ? appRoleAssignment.Id.ToString() : string.Empty;
                }

                // Check the Orders.Manager role
                if (((AppRoleAssignment)appRoleAssignment).AppRoleId.ToString() == _configuration.GetSection("AppRoles:OrdersManager").Value)
                {
                    userRoles.HasOrdersManagerRole = true;
                    userRoles.OrdersManagerRoleAssignmentId = ((AppRoleAssignment)appRoleAssignment).Id != null ? ((AppRoleAssignment)appRoleAssignment).Id.ToString() : string.Empty;
                }
            }
        }

        return userRoles;
    }

    public async Task<IActionResult> GetAsync()
    {

        UserRoles userRoles = new UserRoles();
        try
        {
            userRoles = await GetUserRolesAndGroups();
        }
        catch (ODataError odataError)
        {
            userRoles.ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "GetRolesAndGroupsAsync");
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            userRoles.ErrorMessage = $"Can't read the profile due to the following error: {error}";
            AppInsights.TrackException(_telemetry, ex, "GetRolesAndGroupsAsync");
        }

        return Ok(userRoles);
    }

    [HttpPost]
    public async Task<IActionResult> OnPostAsync([FromForm] UserRoles requestUserRoles)
    {

        UserRoles userRoles = new UserRoles();
        // Get the user unique identifier
        string? userObjectId = User.GetObjectId();

        if (userObjectId == null)
        {
            userRoles.ErrorMessage = "The account cannot be updated since your access token doesn't contain the required 'objectidentifier' claim.";
        }

        GraphServiceClient graphClient = null;

        try
        {
            graphClient = MsalAccessTokenHandler.GetGraphClient(this._configuration);

            // Get current app role assignments and groups
            userRoles = await GetUserRolesAndGroups();

            // Check whether to add or remove
            if (userRoles.MemberOfCommercialAccounts == true && requestUserRoles.MemberOfCommercialAccounts == false)
            {
                // Remove the security group 
                await graphClient.Groups[_configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value].Members[userObjectId].Ref.DeleteAsync();
                userRoles.MemberOfCommercialAccounts = false;
            }
            else if (userRoles.MemberOfCommercialAccounts == false && requestUserRoles.MemberOfCommercialAccounts == true)
            {
                // Add the security group
                var requestBody = new ReferenceCreate
                {
                    OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userObjectId}",
                };
                await graphClient.Groups[_configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value].Members.Ref.PostAsync(requestBody);
                userRoles.MemberOfCommercialAccounts = true;
            }

            if (userRoles.HasOrdersManagerRole == true && requestUserRoles.HasOrdersManagerRole == false)
            {
                // Remove the Orders.Manager application role
                await graphClient.Users[userObjectId].AppRoleAssignments[userRoles.OrdersManagerRoleAssignmentId].DeleteAsync();
                userRoles.HasOrdersManagerRole = false;
            }
            else if (userRoles.HasOrdersManagerRole == false && requestUserRoles.HasOrdersManagerRole == true)
            {
                // Add the Orders.Manager application role 
                var requestBody = new AppRoleAssignment
                {
                    PrincipalId = Guid.Parse(userObjectId),
                    ResourceId = Guid.Parse(_configuration.GetSection("AppRoles:PrincipalId").Value),
                    AppRoleId = Guid.Parse(_configuration.GetSection("AppRoles:OrdersManager").Value)
                };
                var result = await graphClient.Users[userObjectId].AppRoleAssignments.PostAsync(requestBody);
                userRoles.HasOrdersManagerRole = true;
            }


            if (userRoles.HasProductsContributorRole == true && requestUserRoles.HasProductsContributorRole == false)
            {
                // Remove the Products.Contributor application role
                await graphClient.Users[userObjectId].AppRoleAssignments[userRoles.ProductsContributorAssignmentId].DeleteAsync();
                userRoles.HasProductsContributorRole = false;
            }
            else if (userRoles.HasProductsContributorRole == false && requestUserRoles.HasProductsContributorRole == true)
            {
                // Add the Products.Contributor application role 
                var requestBody = new AppRoleAssignment
                {
                    PrincipalId = Guid.Parse(userObjectId),
                    ResourceId = Guid.Parse(_configuration.GetSection("AppRoles:PrincipalId").Value),
                    AppRoleId = Guid.Parse(_configuration.GetSection("AppRoles:ProductsContributor").Value)
                };
                var result = await graphClient.Users[userObjectId].AppRoleAssignments.PostAsync(requestBody);
                userRoles.HasProductsContributorRole = true;
            }
        }
        catch (ODataError odataError)
        {
            userRoles.ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            AppInsights.TrackException(_telemetry, odataError, "OnPostRolesAsync");
        }
        catch (Exception ex)
        {
            string error = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            userRoles.ErrorMessage = $"Can't read the profile due to the following error: {error}";
            AppInsights.TrackException(_telemetry, ex, "OnPostRolesAsync");
        }

        return Ok(userRoles);
    }
}
