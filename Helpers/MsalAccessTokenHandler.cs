using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Drawing;
using Microsoft.Extensions.Caching.Memory;
using Azure.Identity;
using Microsoft.Graph;

namespace woodgrovedemo.Helpers
{
    public class MsalAccessTokenHandler
    {
        public static X509Certificate2 ReadCertificate(string certificateThumbprint)
        {
            if (string.IsNullOrWhiteSpace(certificateThumbprint))
            {
                throw new ArgumentException("certificateThumbprint should not be empty. Please set the certificateThumbprint setting in the appsettings.json", "certificateThumbprint");
            }
            CertificateDescription certificateDescription = CertificateDescription.FromStoreWithThumbprint(
                 certificateThumbprint,
                 StoreLocation.CurrentUser,
                 StoreName.My);

            DefaultCertificateLoader defaultCertificateLoader = new DefaultCertificateLoader();
            defaultCertificateLoader.LoadIfNeeded(certificateDescription);

            if (certificateDescription.Certificate == null)
            {
                throw new Exception("Cannot find the certificate.");
            }

            return certificateDescription.Certificate;
        }

        public static  GraphServiceClient GetGraphClient(IConfiguration configuration, string[] scopes = null)
        {
            string? tenantId = configuration.GetSection("MicrosoftGraph:TenantId").Value;
            string? clientId = configuration.GetSection("MicrosoftGraph:ClientId").Value;
            string? certificateThumbprint = configuration.GetSection("MicrosoftGraph:CertificateThumbprint").Value;

            X509Certificate2 certificate = ReadCertificate(certificateThumbprint);

            var clientCertificateCredential = new ClientCertificateCredential(tenantId, clientId, certificate);

            if (scopes == null)
            {
                scopes = new string[] { "https://graph.microsoft.com/.default" };
            }

            var graphClient = new GraphServiceClient(clientCertificateCredential, scopes);

            return graphClient;
        }

        public static async Task<string> AcquireToken(IConfiguration configuration)
        {
            // Aquire an access token which will be sent as bearer to the request API
            var accessToken = await MsalAccessTokenHandler.GetAccessToken(configuration);
            if (accessToken.Item1 == String.Empty)
            {
                throw new Exception(String.Format("Failed to acquire access token: {0} : {1}", accessToken.error, accessToken.error_description));
            }

            return accessToken.Item1;
        }

        public static async Task<(string token, string error, string error_description)> GetAccessToken(IConfiguration configuration, string[] scopes = null)
        {
            string? tenantId = configuration.GetSection("MicrosoftGraph:TenantId").Value;
            string? clientId = configuration.GetSection("MicrosoftGraph:ClientId").Value;
            string? certificateThumbprint = configuration.GetSection("MicrosoftGraph:CertificateThumbprint").Value;

            // You can run this sample using Certificate. The code will differ only when instantiating the IConfidentialClientApplication
            //string authority = $"{configuration.GetSection("MicrosoftGraph:TenantId").Value!}{configuration.GetSection("MicrosoftGraph:TenantId").Value!}";

            // Since we are using application permissions this will be a confidential client application
            X509Certificate2 certificate = ReadCertificate(certificateThumbprint);
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithCertificate(certificate)
                .WithTenantId(tenantId)
                .Build();

            //configure in memory cache for the access tokens. The tokens are typically valid for 60 seconds,
            //so no need to create new ones for every web request
            app.AddDistributedTokenCache(services =>
            {
                services.AddDistributedMemoryCache();
                services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = Microsoft.Extensions.Logging.LogLevel.Debug);
            });

            // With client credentials flows the scopes is ALWAYS of the shape "resource/.default", as the 
            // application permissions need to be set statically (in the portal or by PowerShell), and then granted by
            // a tenant administrator.  
            if (scopes == null)
            {
                scopes = new string[] { "https://graph.microsoft.com/.default" };
            }

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync();
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected
                return (string.Empty, "500", "Scope provided is not supported");
                //return BadRequest(new { error = "500", error_description = "Scope provided is not supported" });
            }
            catch (MsalServiceException ex)
            {
                // general error getting an access token
                return (String.Empty, "500", "Something went wrong getting an access token for the client API:" + ex.Message);
                //return BadRequest(new { error = "500", error_description = "Something went wrong getting an access token for the client API:" + ex.Message });
            }

            return (result.AccessToken, String.Empty, String.Empty);
        }

    }
}
