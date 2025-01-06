using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniverseStudio.Controllers;

public class SteamControllerFilter : IActionFilter
{
    private readonly bool _isAvailable;
    
    public SteamControllerFilter(IConfiguration configuration)
    {
        _isAvailable = !bool.Parse(configuration["maintenance"]!);
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!_isAvailable)
        {
            context.Result = new ContentResult
            {
                Content = "This feature is temporary unavailable. Please contact our developers if you think something might be wrong.",
                StatusCode = (int) HttpStatusCode.ServiceUnavailable
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}