using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniverseStudio.Controllers;

public class SteamControllerFilter : IActionFilter
{
    private readonly bool isAvailable;
    
    public SteamControllerFilter(IConfiguration configuration)
    {
        isAvailable = !bool.Parse(configuration.GetSection("Controllers")["maintenance"]!);
    }
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!isAvailable)
        {
            context.Result = new ContentResult
            {
                Content = "This features is temporary unavailable. Please contact our developers if you think something might be wrong.",
                StatusCode = (int) HttpStatusCode.ServiceUnavailable
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        throw new NotImplementedException();
    }
}