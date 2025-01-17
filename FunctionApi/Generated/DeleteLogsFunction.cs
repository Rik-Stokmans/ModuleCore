using System.Reflection;
using System.Text.Json;
using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static Authenticator.Authenticator;

namespace FunctionApi.Generated
{
    

    public class DeleteLogsFunction(ILogger<DeleteLogsFunction> logger)
    {
        [Function("DeleteLogs")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
        {
            logger.LogInformation("Processing request for DeleteLogs.");

            // Authenticate the request
            var isAuthenticated = true;
            
            var (permissions, client) = GetAuthenticationPermissions(req.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()));
            
            try
            {
                (typeof(LoggingModule).GetMethod("DeleteLogs") ?? throw new InvalidOperationException()).GetCustomAttributes(typeof(AuthPermissionClaim)).ToList().ForEach(claim =>
                {
                    if (client == "")
                    {
                        isAuthenticated = false;
                    }
                    
                    if (!permissions.Contains((AuthPermissionClaim) claim))
                    {
                        isAuthenticated = false;
                    }
                });
            }
            catch (Exception ex)
            {
                // ignored
            }
            
            if (!isAuthenticated)
            {
                return new UnauthorizedResult();
            }


               
            

            // Call the service method and handle the result
            
            var result = LoggingModule.DeleteLogs(
                #pragma warning disable CS8604 // Possible null reference argument.
                
                #pragma warning restore CS8604 // Possible null reference argument.
            );

            return (result.Code / 100) switch
            {
                2 => new OkObjectResult(result),
                4 => new BadRequestObjectResult(result),
                _ => new StatusCodeResult(result.Code)
            };
                    
        }
    }
}
                