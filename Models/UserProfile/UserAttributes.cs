
namespace woodgrovedemo.Models;
public class UserAttributes
{
    public string ErrorMessage { get; set; } = "";

    /* User attributes*/
    public string DisplayName { get; set; } = "";
    public string? GivenName { get; set; } = "";
    public string? Surname { get; set; } = "";
    public string? Country { get; set; } = "";
    public string? City { get; set; } = "";
    public string? SpecialDiet { get; set; } = "";
    public bool AccountEnabled { get; set; } = true;
    public string? Username { get; set; } = "";
    public string? ObjectId { get; set; } = "";
    public string? CreatedDateTime { get; set; } = "";
    public string? LastPasswordChangeDateTime { get; set; } = "";
}