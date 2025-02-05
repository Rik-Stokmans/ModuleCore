using System.Reflection;
using System.Text.Json;
using LogicLayer.CoreModels;
using LogicLayer.Modules.ChildFocusModule;
using LogicLayer.Modules.ChildFocusModule.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static Authenticator.Authenticator;

namespace FunctionApi.Generated
{
    
     public class LogChildFocusFunctionParameterObject
     {
         public ChildFocusLog ChildFocusLog { get; set; }
     }


    public class LogChildFocusFunction(ILogger<LogChildFocusFunction> logger)
    {
        [Function("LogChildFocus")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            logger.LogInformation("Processing request for LogChildFocus.");

            // Authenticate the request
            var isAuthenticated = true;
            
            var (permissions, client) = GetAuthenticationPermissions(req.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()));
            
            try
            {
                (typeof(ChildFocusModule).GetMethod("LogChildFocus") ?? throw new InvalidOperationException()).GetCustomAttributes(typeof(AuthPermissionClaim)).ToList().ForEach(claim =>
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


               
            LogChildFocusFunctionParameterObject? requestData;
            try
            {
                // Deserialize the request body
                requestData = JsonSerializer.Deserialize<LogChildFocusFunctionParameterObject>(await new StreamReader(req.Body).ReadToEndAsync());
                if (requestData == null)
                {
                    return new BadRequestObjectResult("Request body is null or empty.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse request body.");
                return new BadRequestObjectResult("Invalid JSON in request body.");
            }

            // Validate parameters
            var errors = new List<string>();
           // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
           if (requestData.ChildFocusLog == null)
           {
               errors.Add("Parameter 'ChildFocusLog' is required but was null.");
           }

            if (errors.Any())
            {
                return new BadRequestObjectResult(string.Join("; ", errors));
            }

            // Call the service method and handle the result
            
            var result = ChildFocusModule.LogChildFocus(
                #pragma warning disable CS8604 // Possible null reference argument.
                requestData.ChildFocusLog
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
                