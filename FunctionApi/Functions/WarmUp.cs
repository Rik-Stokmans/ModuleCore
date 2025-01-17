using AzureDatabase.Services;
using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MockDataLayer.Services;

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
            Core.Init(registry =>
            {
                // Register your services here
                registry.Register<ILogService>(new AzureLogService());
                registry.Register<IAuthenticationService>(new AzureAuthenticationService());
                registry.Register<IChildFocus>(new ChildFocusMockService());
                registry.Register<INewsObjectService>(new NewsObjectTransientService());
            });
            
            return new OkResult();
        }
    }
}