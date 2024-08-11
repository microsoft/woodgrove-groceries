
namespace woodgrovedemo.Models;
public class UserRoles
{
    public string ErrorMessage { get; set; } = "";

    /* User attributes*/
    public bool MemberOfCommercialAccounts { get; set; } = false;
    public bool HasOrdersManagerRole { get; set; } = false;
    public bool HasProductsContributorRole { get; set; } = false;
    public string? ProductsContributorAssignmentId { get; set; } = "";
    public string? OrdersManagerRoleAssignmentId { get; set; } = "";
}