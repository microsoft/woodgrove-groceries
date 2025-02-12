using Microsoft.AspNetCore.Authentication.OpenIdConnect;

public class AuthScheme
{
    public const string ArkoseFraudProtection = "ArkoseFraudProtection";
    public const string EmailOtp = "EmailOtp";

    public static string[] All
    {
        get
        {
            return new string[] {
            OpenIdConnectDefaults.AuthenticationScheme,
                AuthScheme.ArkoseFraudProtection,
                AuthScheme.EmailOtp };
        }
    }
}