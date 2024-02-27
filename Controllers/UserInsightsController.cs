using System.Dynamic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;
using woodgrovedemo.Helpers;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserInsightsController : ControllerBase
{
    private const string baseUrl = "https://graph.microsoft.com/beta/reports/userinsights";
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
    /// Generic function to fetch data from Microsoft Graph API
    /// </summary>
    /// <param name="Url">The full ULR of the Graph API endpoint</param>
    /// <returns>The body of the Microsoft Graph API response</returns>
    private async Task<IActionResult> CallGraphAPI(string Url)
    {
        string responseString;
        if (!_memoryCache.TryGetValue(Url, out responseString))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await MsalAccessTokenHandler.AcquireToken(this._configuration));

            var response = await client.GetAsync(Url);
            responseString = await response.Content.ReadAsStringAsync();

            // Add to the cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(1));

            _memoryCache.Set(Url, responseString, cacheEntryOptions);
        }

        return Ok(JsonSerializer.Deserialize<dynamic>(responseString));
    }


    [HttpGet("daily/userCount")]
    public async Task<IActionResult> DailyUserCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/daily/userCount");
    }

    [HttpGet("daily/authentications")]
    public async Task<IActionResult> DailyAuthenticationsCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/daily/authentications");
    }

    [HttpGet("daily/activeUsers")]
    public async Task<IActionResult> DailyActiveUsersCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/daily/activeUsers");
    }

    [HttpGet("daily/inactiveUsers")]
    public async Task<IActionResult> DailyInactiveUsersCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/daily/inactiveUsers");
    }

    [HttpGet("monthly/summary")]
    public async Task<IActionResult> MonthlySummaryAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/summary");
    }

    [HttpGet("monthly/signUps")]
    public async Task<IActionResult> MonthlySignUpsCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/signUps");
    }

    [HttpGet("monthly/activeUsers")]
    public async Task<IActionResult> MonthlyActiveUsersCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/activeUsers");
    }

    [HttpGet("monthly/authentications")]
    public async Task<IActionResult> MonthlyAuthenticationsCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/authentications");
    }

    [HttpGet("monthly/requests")]
    public async Task<IActionResult> DailyRequestsCountAsync()
    {
        return await CallGraphAPI($"{baseUrl}/monthly/requests");
    }
}
