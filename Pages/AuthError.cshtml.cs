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
                this.UserMessage = "Usually this error happens when you sign-in with an account that was deleted. To fix this issue you need to <a href='/SignIn?handler=InvalidSession'>sign-in with this link</a>."; 
            }
        }

    }
}
