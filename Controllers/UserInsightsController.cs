using System.Dynamic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserInsightsController : ControllerBase
{
    private const string baseUrl = "https://graph.microsoft.com/beta/reports/userinsights";
    string[] scopes = { "https://graph.microsoft.com/.default" };
    private readonly IConfiguration _configuration;

    private static readonly HttpClient client = new HttpClient();

    private readonly ILogger<UserInsightsController> _logger;
    private readonly IMemoryCache _memoryCache;


    public UserInsightsController(ILogger<UserInsightsController> logger, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _logger = logger;
        _configuration = configuration;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Get an access token to call the Microsoft Graph API
    /// </summary>
    /// <returns></returns>
    private string GetAccessToken()
    {
        // Get an access token to call the "Account" API (the first API in line)
        string TenantId = _configuration.GetSection("MicrosoftGraph:TenantId").Value!;
        string ClientId = _configuration.GetSection("MicrosoftGraph:ClientId").Value!;
        string ClientSecret = _configuration.GetSection("MicrosoftGraph:ClientSecret").Value!;
        var clientSecretCredential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);

        var at = clientSecretCredential.GetToken(new Azure.Core.TokenRequestContext(
            new[] { "https://graph.microsoft.com/.default" }));

        return at.Token;
    }

    /// <summary>
    /// Generic function to fetch data from Microsoft Graph API
    /// </summary>
    /// <param name="Url">The full ULR of the Graph API endpoint</param>
    /// <returns>The body of the Microsoft Graph API response</returns>
    private async Task<IActionResult> CallGraphAPI(string Url)
    {
        string responseString;
        if (!_memoryCache.TryGetValue(Url, out responseString))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());

            var response = await client.GetAsync(Url);
            responseString = await response.Content.ReadAsStringAsync();

            // Add to the cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(1));

            _memoryCache.Set(Url, responseString, cacheEntryOptions);
        }

        return Ok(JsonSerializer.Deserialize<dynamic>(responseString));
    }

    [HttpGet("monthly/summary")]
    public async Task<IActionResult> MonthlySummaryAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/summary");
    }

    [HttpGet("monthly/activeUsers")]
    public async Task<IActionResult> MonthlyActiveUsersCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/signUps");
    }

    [HttpGet("daily/authentications")]
    public async Task<IActionResult> DailyAuthenticationsCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/daily/authentications");
    }

    [HttpGet("monthly/signUps")]
    public async Task<IActionResult> MonthlySignUpsCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/signUps");
    }

}
