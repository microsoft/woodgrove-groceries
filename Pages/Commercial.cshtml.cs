using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{
    [Authorize(Policy = "CommercialOnly")]
    public class CommercialModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
