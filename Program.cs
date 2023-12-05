using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// The following line enables Application Insights telemetry collection.
builder.Services.AddApplicationInsightsTelemetry();

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
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();

async Task OnRedirectToIdentityProviderFunc(RedirectContext context)
{
    // Read the custom parameter
    var forceSignIn = context.Properties.Items.FirstOrDefault(x => x.Key == "force").Value;

    // Add your custom code here
    if (forceSignIn != null)
    {
        context.ProtocolMessage.Prompt = "login";
    }

    // Don't remove this line
    await Task.CompletedTask.ConfigureAwait(false);
}