using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using woodgrovedemo.Helpers;
using Microsoft.Extensions.Logging;

namespace woodgrovedemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemosController : ControllerBase
{
    private readonly ILogger<DemosController> _logger;

    public DemosController(ILogger<DemosController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<List<DemoData>> Get()
    {
        try
        {
            _logger.LogInformation("Demo endpoint called at {timestamp}", DateTime.UtcNow);

            // Retrieve demo data
            List<DemoData> demoData = DemoDataList.Demos;

            // Filter only demos that are not server-side only
            demoData = demoData.Where(d => !d.ServerSideOnly).ToList();

            // Return the filtered demo data    
            return Ok(demoData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in Demo endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
