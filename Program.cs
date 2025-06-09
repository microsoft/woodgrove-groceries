using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();
var _telemetry = builder.Services.BuildServiceProvider().GetService<TelemetryClient>();

// Initialize the demo data
DemoDataList.Initialize(builder.Configuration);

// Add Microsoft Identity Web App authentication
ConfigurationSection AzureAd = (ConfigurationSection)builder.Configuration.GetSection("AzureAd");
ConfigurationSection WoodgroveGroceriesApi = (ConfigurationSection)builder.Configuration.GetSection("WoodgroveGroceriesApi");

// Avoid mapping of claims from short name to long (SAML like) claims.
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Default scheme sign-in flow
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(AzureAd, OpenIdConnectDefaults.AuthenticationScheme)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("GraphApiMiddleware", WoodgroveGroceriesApi)
    .AddDownstreamApi("WoodgroveGroceriesApi", WoodgroveGroceriesApi)
    .AddMicrosoftGraph(builder.Configuration.GetSection("GraphApi"))
    .AddInMemoryTokenCaches();

// ArkoseFraudProtection scheme sign-in flow
ConfigurationSection ArkoseFraudProtection = (ConfigurationSection)builder.Configuration.GetSection(AuthScheme.ArkoseFraudProtection);
builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApp(ArkoseFraudProtection, AuthScheme.ArkoseFraudProtection, AuthScheme.ArkoseFraudProtection + "Cookies")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("WoodgroveGroceriesApiInvite", WoodgroveGroceriesApi)
    .AddMicrosoftGraph(builder.Configuration.GetSection("GraphApi"))
    .AddInMemoryTokenCaches();

// EmailOtp scheme sign-in flow
ConfigurationSection EmailOtp = (ConfigurationSection)builder.Configuration.GetSection(AuthScheme.EmailOtp);
builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApp(EmailOtp, AuthScheme.EmailOtp, AuthScheme.EmailOtp + "Cookies")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("WoodgroveGroceriesApiInvite", WoodgroveGroceriesApi)
    .AddMicrosoftGraph(builder.Configuration.GetSection("GraphApi"))
    .AddInMemoryTokenCaches();

// Set authentication options for all schemes
foreach (var scheme in AuthScheme.All)
{
    builder.Services.Configure<OpenIdConnectOptions>(scheme,
              options =>
              {
                  options.TokenValidationParameters.RoleClaimType = "roles";
                  options.TokenValidationParameters.NameClaimType = "name";
                  options.Events.OnRedirectToIdentityProvider += OnRedirectToIdentityProviderFunc;
                  options.Events.OnMessageReceived += OnMessageReceivedFunc;
                  options.Events.OnAuthenticationFailed += OnAuthenticationFailedFunc;
                  options.Events.OnRemoteFailure += OnRemoteFailureFunc;
                  options.Events.OnTokenValidated += OnTokenValidatedFunc;
                  options.RemoteAuthenticationTimeout = TimeSpan.FromMinutes(30);
                  options.SaveTokens = true;

                  options.ForwardDefaultSelector = context =>
                    {
                        string scheme = OpenIdConnectDefaults.AuthenticationScheme;

                        // Check the scheme from the cookies (if exists)
                        // This check is required for the sign-in postback and sign-out flows
                        foreach (var item in context.Request.Cookies)
                        {
                            if (item.Key == ".AspNetCore.ArkoseFraudProtectionCookies")
                            {
                                scheme = AuthScheme.ArkoseFraudProtection;
                                break;
                            }
                            else if (item.Key == ".AspNetCore.EmailOtpCookies")
                            {
                                scheme = AuthScheme.EmailOtp;
                                break;
                            }
                        }

                        string? handler = context.Request.Query["handler"];

                        // Force change the scheme of explicitly requested by the sign-in
                        if (handler != null && (handler == AuthScheme.ArkoseFraudProtection ||
                            handler == AuthScheme.EmailOtp))
                        {
                            scheme = handler;
                        }

                        return scheme;
                    };
              });
}


builder.Services.AddAuthentication();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIdConnectDefaults.AuthenticationScheme; //"DynamicAuth";
});
// .AddPolicyScheme("DynamicAuth", "Automatically Select Scheme", options =>
// {
//     options.ForwardDefaultSelector = context =>
//     {
//         string scheme = OpenIdConnectDefaults.AuthenticationScheme;

//         // Check the scheme from the cookies (if exists)
//         // This check is required for the sign-in postback and sign-out flows
//         foreach (var item in context.Request.Cookies)
//         {
//             if (item.Key == ".AspNetCore.ArkoseFraudProtectioncookies")
//             {
//                 scheme = AuthScheme.ArkoseFraudProtection;
//                 break;
//             }
//         }

//         string? handler = context.Request.Query["handler"];

//         // Force change the scheme of explicitly requested by the sign-in
//         if (handler != null && handler == AuthScheme.ArkoseFraudProtection)
//         {
//             scheme = handler;
//         }

//         return scheme;
//     };
// });

builder.Services.AddAuthorization(options =>
{

    // Get the commercial accounts security group ID
    string commercialAccountsSecurityGroup = ((ConfigurationSection)builder.Configuration.GetSection("AppRoles:CommercialAccountsSecurityGroup")).Value!;

    // Check whether the account is a member of the commercial accounts security group
    options.AddPolicy("CommercialOnly", policy => policy.RequireClaim("groups", commercialAccountsSecurityGroup));

    // Get the exclusive demos security group ID
    string exclusiveDemosSecurityGroup = ((ConfigurationSection)builder.Configuration.GetSection("AppRoles:ExclusiveDemosSecurityGroup")).Value!;

    // Check whether the account is a member of the exclusive demos security group
    options.AddPolicy("ExclusiveDemosOnly", policy => policy.RequireClaim("groups", exclusiveDemosSecurityGroup));

    // Loyalty authorization policy
    options.AddPolicy("LoyaltyAccess", policy =>
        policy.RequireAssertion(context =>

            // Verify whether the loyalty number or loyalty tier claims are included within the security token.
            context.User.HasClaim(c => (c.Type == "loyaltyTier" || c.Type == "loyaltyNumber"))
            &&
            // Verify whether the user has been enrolled in the loyalty program for at least one month.
            context.User.Claims.Any(c => c.Type == "loyaltySince" && DateTime.UtcNow.AddMonths(-1) >= DateTime.Parse(c.Value))
        ));

});



builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();


async Task OnTokenValidatedFunc(TokenValidatedContext context)
{

    // Add the scheme name claim for non standard scenario
    var claim = new Claim("AuthScheme", context.Scheme.Name);
    var identity = new ClaimsIdentity(new[] { claim });
    context.Principal.AddIdentity(identity);

    await Task.CompletedTask;
}

async Task OnRedirectToIdentityProviderFunc(RedirectContext context)
{
    // Read the 'force' custom parameter
    var forceSignIn = context.Properties.Items.FirstOrDefault(x => x.Key == "force").Value;

    // Add your custom code here
    if (forceSignIn != null)
    {
        context.ProtocolMessage.Prompt = "login";
    }

    // Read the 'StepUp' custom parameter
    var stepUp = context.Properties.Items.FirstOrDefault(x => x.Key == "StepUp").Value;

    // Add your custom code here
    if (stepUp != null)
    {
        context.ProtocolMessage.Parameters.Add("claims", "%7B%22access_token%22%3A%7B%22acrs%22%3A%7B%22essential%22%3Atrue%2C%22value%22%3A%22c1%22%7D%7D%7D"); ;
    }

    // Read the 'StepUp' custom parameter
    var domain = context.Properties.Items.FirstOrDefault(x => x.Key == "domain").Value;

    // Add your custom code here
    if (domain != null)
    {
        var builder = new UriBuilder(context.ProtocolMessage.IssuerAddress);
        builder.Host = domain;
        context.ProtocolMessage.IssuerAddress = builder.Uri.ToString();
    }

    // Read the 'prompt' custom parameter
    var prompt = context.Properties.Items.FirstOrDefault(x => x.Key == "prompt").Value;
    if (prompt != null)
    {
        context.ProtocolMessage.Prompt = prompt;
    }

    // Read the 'ui_locales' custom parameter
    var ui_locales = context.Properties.Items.FirstOrDefault(x => x.Key == "ui_locales").Value;

    if (ui_locales != null)
    {
        context.ProtocolMessage.Parameters.Add("mkt", ui_locales);
        context.ProtocolMessage.UiLocales = ui_locales;
    }

    // Read the 'login_hint' custom parameter
    var login_hint = context.Properties.Items.FirstOrDefault(x => x.Key == "login_hint").Value;

    if (login_hint != null)
    {
        context.ProtocolMessage.LoginHint = login_hint;
    }

    // Read the 'domain_hint' custom parameter
    var domain_hint = context.Properties.Items.FirstOrDefault(x => x.Key == "domain_hint").Value;

    if (domain_hint != null)
    {
        context.ProtocolMessage.DomainHint = domain_hint;
    }
    
    // Read the 'query-string' custom query string
    var queryString = context.Properties.Items.FirstOrDefault(x => x.Key == "query-string").Value;

    if (queryString != null)
    {
        string[] parmas = queryString.Split("&");
        foreach (var parma in parmas)
        {
            string[] kv = parma.Split("=");
            if (kv.Length == 2)
            {
                context.ProtocolMessage.Parameters.Add(kv[0], kv[1]);
            }
        }
    }
    // Don't remove this line
    await Task.CompletedTask.ConfigureAwait(false);
}

// Invoked when an OpenIdConnect message is first received.
async Task OnMessageReceivedFunc(MessageReceivedContext context)
{
    if (context.ProtocolMessage != null && context.ProtocolMessage.Error != null)
    {
        if (_telemetry != null)
        {
            PageViewTelemetry pageView = new PageViewTelemetry("AuthError");

            // Track the error into the authentication error page
            pageView.Properties.Add("Error", context.ProtocolMessage.Error);
            pageView.Properties.Add("ErrorDescription", context.ProtocolMessage.ErrorDescription);
            pageView.Properties.Add("ErrorUri", context.ProtocolMessage.ErrorUri);

            int errorCode = context.ProtocolMessage.ErrorDescription.IndexOf(":");
            if (errorCode <= 12)
            {
                pageView.Properties.Add("ErrorCode", context.ProtocolMessage.ErrorDescription.Substring(0, errorCode));
            }

            _telemetry.TrackPageView(pageView);
        }

        context.HandleResponse();
        context.Response.Redirect($"/AuthError?error={context.ProtocolMessage.Error}&description={context.ProtocolMessage.ErrorDescription}");
        await Task.CompletedTask.ConfigureAwait(false);
    }
}

// Invoked if exceptions are thrown during OpenIdConnect request processing. 
// The exceptions will be re-thrown after this event unless suppressed.
async Task OnAuthenticationFailedFunc(AuthenticationFailedContext context)
{
    if (_telemetry != null)
    {
        PageViewTelemetry pageView = new PageViewTelemetry("AuthError");

        // Track the error into the authentication error page
        pageView.Properties.Add("Error", "AuthenticationFailed");
        pageView.Properties.Add("ErrorDescription", context.Exception.Message);
        pageView.Properties.Add("ErrorCode", "APP_AUTH_0001");

        _telemetry.TrackPageView(pageView);
    }

    context.HandleResponse();
    context.Response.Redirect($"/AuthError?error=APP_AUTH_0001&description={context.Exception.Message}");
    await Task.CompletedTask.ConfigureAwait(false);
}

// Invoked when there is an OpenIdConnect remote failure.
async Task OnRemoteFailureFunc(RemoteFailureContext context)
{
    if (_telemetry != null)
    {
        PageViewTelemetry pageView = new PageViewTelemetry("AuthError");

        // Track the error into the authentication error page
        pageView.Properties.Add("Error", "RemoteFailure");
        pageView.Properties.Add("ErrorDescription", context.Failure.Message);
        pageView.Properties.Add("ErrorCode", "APP_AUTH_0002");

        _telemetry.TrackPageView(pageView);
    }

    context.HandleResponse();
    context.Response.Redirect($"/AuthError?error=APP_AUTH_0002&description={context.Failure.Message}");
    await Task.CompletedTask.ConfigureAwait(false);
}


