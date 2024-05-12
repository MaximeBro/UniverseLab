using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace UpdateAPI;

[Route("/api/version/")]
public class UpdateController(IConfiguration configuration) : Controller
{
    private bool Maintenance => (configuration["maintenance"] ?? string.Empty).ToLower() == "true";
    
    [HttpGet("Version")]
    public IActionResult GetAppVersion()
    {
        if (Maintenance)
        {
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, "This features is temporary unavailable. Please contact our developers if you think something might be wrong.");
        }

        return Ok(configuration["app-version"]);
    }

    [HttpGet("Maintenance")]
    public IActionResult GetMaintenance()
    {
        return Ok(Maintenance);
    }
}