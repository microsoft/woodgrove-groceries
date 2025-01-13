
namespace woodgrovedemo.Models;
public class UserMoreInfo
{
    public string ErrorMessage { get; set; } = "";

    /* User attributes*/
    public string Identities { get; set; } = "";
    public string LastSignInDateTime { get; set; } = "";
    public string LastSignInRequestId { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string EmailMfa { get; set; } = "";
    public string SingInEmail { get; set; } = "";
}