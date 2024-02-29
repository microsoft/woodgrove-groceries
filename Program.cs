using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();
var _telemetry = builder.Services.BuildServiceProvider().GetService<TelemetryClient>();

ConfigurationSection AzureAd = (ConfigurationSection)builder.Configuration.GetSection("AzureAd");
ConfigurationSection WoodgroveGroceriesApi = (ConfigurationSection)builder.Configuration.GetSection("WoodgroveGroceriesApi");

// Avoid mapping of claims from short name to long (SAML like) claims.
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Default sign-in flow
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(AzureAd, OpenIdConnectDefaults.AuthenticationScheme)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("WoodgroveGroceriesApi", WoodgroveGroceriesApi)
    .AddInMemoryTokenCaches();

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme,
                                                 options =>
                                                 {
                                                     options.TokenValidationParameters.RoleClaimType = "roles";
                                                     options.TokenValidationParameters.NameClaimType = "name";
                                                     options.Events.OnRedirectToIdentityProvider += OnRedirectToIdentityProviderFunc;
                                                     options.Events.OnMessageReceived += OnMessageReceivedFunc;
                                                     options.RemoteAuthenticationTimeout = TimeSpan.FromMinutes(30);
                                                 });



builder.Services.AddAuthentication();

// builder.Services.AddAuthorization(options =>
// {
//     // By default, all incoming requests will be authorized according to the default policy.
//     options.FallbackPolicy = options.DefaultPolicy;
// });

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



    // Don't remove this line
    await Task.CompletedTask.ConfigureAwait(false);
}


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
            if (errorCode <= 12 )
            {
                pageView.Properties.Add("ErrorCode",  context.ProtocolMessage.ErrorDescription.Substring(0, errorCode));
            }

            _telemetry.TrackPageView(pageView);
        }

        context.HandleResponse();
        context.Response.Redirect($"/AuthError?error={context.ProtocolMessage.Error}&description={context.ProtocolMessage.ErrorDescription}");
        await Task.CompletedTask.ConfigureAwait(false);
    }
}



