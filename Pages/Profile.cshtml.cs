using System.ComponentModel;
using Azure.Identity;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace MyApp.Namespace
{

    [Authorize]
    public class ProfileModel : PageModel
    {
        // Dependency injection
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;
        readonly IAuthorizationHeaderProvider _authorizationHeaderProvider;

        // Graph API settings
        private readonly string[] scopes = new[] { "https://graph.microsoft.com/.default" };
        private string TenantId { get; set; } = "";
        private string ClientId { get; set; } = "";
        private string ClientSecret { get; set; } = "";
        private string ExtensionAttributes { get; set; } = "";

        // User interface messages
        public string ErrorMessage { get; private set; } = "";
        public bool UserNeedsToSignInAgain { get; private set; } = false;
        public bool UserNeedsToSignInAgainAfterSignUp { get; private set; } = false;

        /* User attributes*/
        [BindProperty]
        public string DisplayName { get; set; } = "";
        [BindProperty]
        public string GivenName { get; set; } = "";
        [BindProperty]
        public string Surname { get; set; } = "";
        [BindProperty]
        public string Country { get; set; } = "";
        [BindProperty]
        public string City { get; set; } = "";
        [BindProperty]
        public string SpecialDiet { get; set; } = "";
        [BindProperty]
        public string Identities { get; private set; } = "";
        [BindProperty]
        public string ObjectId { get; private set; } = "";


        [DisplayName("Member of 'Commercial Accounts' group")]
        public bool MemberOfCommercialAccounts { get; private set; } = false;
        public bool HasOrdersManagerRole { get; private set; } = false;
        private string OrdersManagerRoleAssignmentId { get; set; } = "";

        public bool HasProductsContributorRole { get; private set; } = false;
        private string ProductsContributorAssignmentId { get; set; } = "";

        // Sign in activity
        public string CreatedDateTime { get; set; } = "";
        public string LastSignInDateTime { get; set; } = "";
        public string LastSignInRequestId { get; set; } = "";
        public string LastPasswordChangeDateTime { get; set; } = "";

        public ProfileModel(IConfiguration configuration, TelemetryClient telemetry, IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            Configuration = configuration;
            _telemetry = telemetry;

            // Get the app settings
            TenantId = Configuration.GetSection("MicrosoftGraph:TenantId").Value!;
            ClientId = Configuration.GetSection("MicrosoftGraph:ClientId").Value!;
            ClientSecret = Configuration.GetSection("MicrosoftGraph:ClientSecret").Value!;
            ExtensionAttributes = Configuration.GetSection("MicrosoftGraph:ExtensionAttributes").Value!;
            _authorizationHeaderProvider = authorizationHeaderProvider;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _telemetry.TrackPageView("Profile:Read");

            // Get the user unique identifier
            string? userObjectId = User.GetObjectId();

            if (userObjectId == null)
            {
                ErrorMessage = "The account cannot be updated since your access token doesn't contain the required 'objectidentifier' claim.";
            }

            await ReadProfile(userObjectId);

            return Page();
        }

        private async Task ReadProfile(string userObjectId)
        {
            try
            {
                var clientSecretCredential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                User profile = await graphClient.Users[userObjectId].GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities", "displayName", "GivenName", "Surname", "Country", "City", "CreatedDateTime", "lastPasswordChangeDateTime", "signInActivity", $"{ExtensionAttributes}_SpecialDiet" };
                        requestConfiguration.QueryParameters.Expand = new string[] { "Extensions" };
                    });


                // Populate the user's attributes
                this.ObjectId = profile!.Id!;
                this.DisplayName = profile!.DisplayName ?? "";
                this.Surname = profile!.Surname ?? "";
                this.GivenName = profile!.GivenName ?? "";
                this.Country = profile!.Country ?? "";
                this.City = profile!.City ?? "";

                // Get the special diet from the extension attributes
                object specialDiet;
                if (profile.AdditionalData.TryGetValue($"{ExtensionAttributes}_SpecialDiet", out specialDiet))
                {
                    this.SpecialDiet = specialDiet.ToString();
                }

                // Get the user identities
                GetIdentities(profile!.Identities!);

                // Get the account creation time
                if (profile.CreatedDateTime != null)
                {
                    this.CreatedDateTime = profile.CreatedDateTime.ToString();
                }

                // Get the sign-in activity
                if (profile.SignInActivity != null)
                {
                    this.LastSignInDateTime = profile!.SignInActivity!.LastSignInDateTime!.ToString()!;
                    this.LastSignInRequestId = profile!.SignInActivity.LastSignInRequestId!;
                }
                else
                {
                    this.LastSignInDateTime = "Data is not yet available.";
                    this.LastSignInRequestId = this.LastSignInDateTime;
                }

                // Get the last time user changed their password
                if (profile.LastPasswordChangeDateTime != null)
                {
                    this.LastPasswordChangeDateTime = profile.LastPasswordChangeDateTime.ToString();
                }
                else
                {
                    this.LastPasswordChangeDateTime = "Data is not available. It might be because you sign-in with a federated account, or email and one time passcode.";
                }

                // Get user's roles and security groups
                await GetRolesAndGroupsAsync(graphClient, userObjectId);

            }
            catch (ODataError odataError)
            {
                ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Can't read the profile due to the following error: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostProfileAsync()
        {
            _telemetry.TrackPageView("Profile:Update");

            // Get the user unique identifier
            string? userObjectId = User.GetObjectId();

            if (userObjectId == null)
            {
                ErrorMessage = "The account cannot be updated since your access token doesn't contain the required 'objectidentifier' claim.";
            }

            var clientSecretCredential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            try
            {
                // Update user by object ID
                var requestBody = new User
                {
                    DisplayName = this.DisplayName,
                    GivenName = this.GivenName,
                    Surname = this.Surname,
                    Country = this.Country,
                    City = this.City,
                    AdditionalData = new Dictionary<string, object>
                    {
                        {
                            $"{ExtensionAttributes}_SpecialDiet" , this.SpecialDiet
                        },
                    }
                };

                var result = await graphClient.Users[userObjectId].PatchAsync(requestBody);

                // Get user's roles and security groups
                await GetRolesAndGroupsAsync(graphClient, userObjectId);

                UserNeedsToSignInAgain = true;
            }
            catch (ODataError odataError)
            {
                ErrorMessage = $"The account cannot be updated due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"The account cannot be deleted due to the following error: {ex.Message}";
            }

            try
            {
                User profile = await graphClient.Users[userObjectId].GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "Id", "identities" };
                });

                // Get the user identities
                GetIdentities(profile!.Identities!);
            }
            catch (System.Exception)
            {

                throw;
            }

            if (UserNeedsToSignInAgain)
            {
                this.UserNeedsToSignInAgainAfterSignUp = await CheckSignUpIssue();
            }

            return Page();
        }



        public async Task<IActionResult> OnPostRolesAsync(bool hasProductsContributorRole,
                                      bool hasOrdersManagerRole,
                                      bool memberOfCommercialAccounts)
        {
            // Check whether the user logged-in
            if (User.Identity?.IsAuthenticated == false)
            {
                return Page();
            }

            // Get the user unique identifier
            string? userObjectId = User.GetObjectId();

            if (userObjectId == null)
            {
                ErrorMessage = "The account cannot be updated since your access token doesn't contain the required 'objectidentifier' claim.";
            }

            var clientSecretCredential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            await GetRolesAndGroupsAsync(graphClient, userObjectId);

            try
            {
                if (MemberOfCommercialAccounts == true && memberOfCommercialAccounts == false)
                {
                    // Remove the security group 
                    await graphClient.Groups[Configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value].Members[userObjectId].Ref.DeleteAsync();
                    MemberOfCommercialAccounts = false;
                }
                else if (MemberOfCommercialAccounts == false && memberOfCommercialAccounts == true)
                {
                    // Add the security group
                    var requestBody = new Microsoft.Graph.Models.ReferenceCreate
                    {
                        OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userObjectId}",
                    };
                    await graphClient.Groups[Configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup").Value].Members.Ref.PostAsync(requestBody);
                    MemberOfCommercialAccounts = true;
                }


                if (HasOrdersManagerRole == true && hasOrdersManagerRole == false)
                {
                    // Remove the Orders.Manager application role
                    await graphClient.Users[userObjectId].AppRoleAssignments[this.OrdersManagerRoleAssignmentId].DeleteAsync();
                    HasOrdersManagerRole = false;
                }
                else if (HasOrdersManagerRole == false && hasOrdersManagerRole == true)
                {
                    // Add the Orders.Manager application role 
                    var requestBody = new AppRoleAssignment
                    {
                        PrincipalId = Guid.Parse(userObjectId),
                        ResourceId = Guid.Parse(Configuration.GetSection("AppRoles:PrincipalId").Value),
                        AppRoleId = Guid.Parse(Configuration.GetSection("AppRoles:OrdersManager").Value)
                    };
                    var result = await graphClient.Users[userObjectId].AppRoleAssignments.PostAsync(requestBody);
                    HasOrdersManagerRole = true;
                }



                if (HasProductsContributorRole == true && hasProductsContributorRole == false)
                {
                    // Remove the Products.Contributor application role
                    await graphClient.Users[userObjectId].AppRoleAssignments[this.ProductsContributorAssignmentId].DeleteAsync();
                    HasProductsContributorRole = false;
                }
                else if (HasProductsContributorRole == false && hasProductsContributorRole == true)
                {
                    // Add the Products.Contributor application role 
                    var requestBody = new AppRoleAssignment
                    {
                        PrincipalId = Guid.Parse(userObjectId),
                        ResourceId = Guid.Parse(Configuration.GetSection("AppRoles:PrincipalId").Value),
                        AppRoleId = Guid.Parse(Configuration.GetSection("AppRoles:ProductsContributor").Value)
                    };
                    var result = await graphClient.Users[userObjectId].AppRoleAssignments.PostAsync(requestBody);
                    HasProductsContributorRole = true;
                }

                UserNeedsToSignInAgain = true;
            }
            catch (ODataError odataError)
            {
                ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Can't read the profile due to the following error: {ex.Message}";
            }

            await ReadProfile(userObjectId);

            if (UserNeedsToSignInAgain)
            {
                this.UserNeedsToSignInAgainAfterSignUp = await CheckSignUpIssue();
            }

            return Page();
        }

        private void GetIdentities(List<ObjectIdentity> identities)
        {
            foreach (var identity in identities!)
            {
                if (identity.SignInType == "userPrincipalName")
                {
                    continue;
                }
                else
                {
                    this.Identities += $"<b>Sign-in type:</b> {identity.SignInType} <b>Issuer</b>: {identity.Issuer} <b>ID</b>: {identity.IssuerAssignedId} <br/>";
                }
            }
        }

        private async Task GetRolesAndGroupsAsync(GraphServiceClient graphClient, string userObjectId)
        {
            try
            {
                var groups = await graphClient.Users[userObjectId].MemberOf.GetAsync();
                foreach (var group in groups.Value)
                {

                    if (((Microsoft.Graph.Models.Group)group).DisplayName == "Commercial accounts")
                    {
                        MemberOfCommercialAccounts = true;
                        break;
                    }
                }

                var appRoleAssignments = await graphClient.Users[userObjectId].AppRoleAssignments.GetAsync();
                foreach (var appRoleAssignment in appRoleAssignments.Value)
                {

                    // Check the Products.Contributor role
                    if (((Microsoft.Graph.Models.AppRoleAssignment)appRoleAssignment).AppRoleId.ToString() == Configuration.GetSection("AppRoles:ProductsContributor").Value)
                    {
                        HasProductsContributorRole = true;
                        ProductsContributorAssignmentId = ((Microsoft.Graph.Models.AppRoleAssignment)appRoleAssignment).Id.ToString();
                    }

                    // Check the Orders.Manager role
                    if (((Microsoft.Graph.Models.AppRoleAssignment)appRoleAssignment).AppRoleId.ToString() == Configuration.GetSection("AppRoles:OrdersManager").Value)
                    {
                        HasOrdersManagerRole = true;
                        OrdersManagerRoleAssignmentId = ((Microsoft.Graph.Models.AppRoleAssignment)appRoleAssignment).Id.ToString();
                    }

                }
            }
            catch (ODataError odataError)
            {
                ErrorMessage = $"Can't read the profile due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Can't read the profile due to the following error: {ex.Message}";
            }
        }

        private async Task<bool> CheckSignUpIssue()
        {
            try
            {
                // Workaround for the sign-up issue
                string[] ApiScopes = this.Configuration.GetSection("WoodgroveGroceriesApi:Scopes").Get<string[]>();
                await _authorizationHeaderProvider.CreateAuthorizationHeaderForUserAsync(ApiScopes);
            }
            catch (System.Exception)
            {

                return true;
            }

            return false;
        }

    }
}

