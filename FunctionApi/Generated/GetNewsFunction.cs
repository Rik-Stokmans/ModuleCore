using System.Reflection;
using System.Text.Json;
using LogicLayer.CoreModels;
using LogicLayer.Modules.NewsScraperModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static Authenticator.Authenticator;

namespace FunctionApi.Generated
{
    
     public class GetNewsFunctionParameterObject
     {
         public String Category { get; set; }
     }


    public class GetNewsFunction(ILogger<GetNewsFunction> logger)
    {
        [Function("GetNews")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            logger.LogInformation("Processing request for GetNews.");

            // Authenticate the request
            var isAuthenticated = true;
            
            var (permissions, client) = GetAuthenticationPermissions(req.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()));
            
            try
            {
                (typeof(NewsScraperModule).GetMethod("GetNews") ?? throw new InvalidOperationException()).GetCustomAttributes(typeof(AuthPermissionClaim)).ToList().ForEach(claim =>
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


               
            GetNewsFunctionParameterObject? requestData;
            try
            {
                // Deserialize the request body
                requestData = JsonSerializer.Deserialize<GetNewsFunctionParameterObject>(await new StreamReader(req.Body).ReadToEndAsync());
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
           if (requestData.Category == null)
           {
               errors.Add("Parameter 'Category' is required but was null.");
           }

            if (errors.Any())
            {
                return new BadRequestObjectResult(string.Join("; ", errors));
            }

            // Call the service method and handle the result
            
             var (result, data) = NewsScraperModule.GetNews(
                 requestData.Category
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
                