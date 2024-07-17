
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.ApplicationInsights;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using woodgrovedemo.Helpers;

namespace MyApp.Namespace
{
    public class DeleteMyAccountModel : PageModel
    {
        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;
        public string ErrorMessage { get; private set; } = "";
        public string Message { get; private set; } = "";

        public DeleteMyAccountModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("ProfileDelete:Read");

            return Page();
        }
        
        [Authorize]
        public async Task OnPostAsync()
        {

            _telemetry.TrackPageView("ProfileDelete:Delete");

            // Get the user unique identifier
            string? userObjectId = User.GetObjectId();

            if (userObjectId == null)
            {
                ErrorMessage = "The account cannot be delete since your access token doesn't contain the required 'objectidentifier' claim.";
            }

            // Get the app settings
            var graphClient = MsalAccessTokenHandler.GetGraphClient(this.Configuration);

            try
            {

                // Delete user by object ID
                await graphClient.Users[userObjectId]
                    .DeleteAsync();

                // Delete the object from the recycle bin
                await graphClient.Directory.DeletedItems[userObjectId].DeleteAsync();


                Message = "Your account has been successfully deleted. Please sign-out from the application";
            }
            catch (ODataError odataError)
            {
                ErrorMessage = $"The account cannot be delete due to the following error: {odataError.Error!.Message} Error code: {odataError.Error.Code}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"The account cannot be delete due to the following error: {ex.Message}";
            }

            return;
        }
    }
}
