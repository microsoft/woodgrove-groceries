using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    public class AuthErrorModel : PageModel
    {

        public string Error { get; set; }
        public string Message { get; set; }
        public string UserMessage { get; set; }

        public void OnGet(string error = "", string description = "")
        {
            this.Error = error;
            this.Message = description;

            if (this.Message.StartsWith("AADSTS16000"))
            {
                this.UserMessage = "Usually this error happens when you sign-in with an account that was deleted, but haven't sign-out from Entra ID. To fix this issue you need to <a href='/SignIn?handler=InvalidSession'>sign-in with this link</a>. The link will invalidate the single sign-on (SSO) session and start a fresh sign-in flow.";
            }
            else if (this.Message.StartsWith("AADSTS50000"))
            {
                this.UserMessage = "There are serveral causes for this error to happen .Usually this error happens when you sign-in with an account that was deleted, but haven't sign-out from Entra ID. To fix this issue you need to <a href='/SignIn?handler=InvalidSession'>sign-in with this link</a>. The link will invalidate the single sign-on (SSO) session and start a fresh sign-in flow.";
            }
            else if (error == "APP_AUTH_0002" && this.Message.Contains("message.State is null or empty"))
            {
                this.UserMessage = "The 'state' parameter is required. Looks like it was removed from the authentication request. To fix this issue, start the demo again.";
            }

            
        }

    }
}
