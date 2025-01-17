using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApi.Functions;

public class WarmUp(ILogger<WarmUp> logger)
{
    [Function("WarmUp")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("Processing request for Login.");

        try
        {
            var service = Core.GetService<IAuthenticationService>();

            return new AcceptedResult();
        }
        catch
        {
            Startup.startup();
            
            return new OkResult();
        }
    }
}