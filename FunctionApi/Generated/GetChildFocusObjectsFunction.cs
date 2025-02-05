using System.Reflection;
using LogicLayer.CoreModels;
using LogicLayer.Modules.ChildFocusModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static Authenticator.Authenticator;

namespace FunctionApi.Generated
{
    

    public class GetChildFocusObjectsFunction(ILogger<GetChildFocusObjectsFunction> logger)
    {
        [Function("GetChildFocusObjects")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            logger.LogInformation("Processing request for GetChildFocusObjects.");

            // Authenticate the request
            var isAuthenticated = true;
            
            var (permissions, client) = GetAuthenticationPermissions(req.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()));
            
            try
            {
                (typeof(ChildFocusModule).GetMethod("GetChildFocusObjects") ?? throw new InvalidOperationException()).GetCustomAttributes(typeof(AuthPermissionClaim)).ToList().ForEach(claim =>
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
            
             var (result, data) = ChildFocusModule.GetChildFocusObjects(
                 
             );

             return result.Code switch
             {
                 >= 200 and < 300 => new OkObjectResult(data),
                 >= 400 and < 500 => new BadRequestObjectResult(result),
                 _ => new StatusCodeResult(result.Code)
             };
                    
        }
    }
}
                