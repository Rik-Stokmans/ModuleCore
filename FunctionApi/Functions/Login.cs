using System.Net;
using System.Text.Json;
using LogicLayer.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OkObjectResult = Microsoft.AspNetCore.Mvc.OkObjectResult;

namespace FunctionApi.Functions;


public class LoginUserObject
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class BearerTokenObject(string bearer, DateTime expires)
{
    public string Bearer { get; set; } = bearer;

    public DateTime Expires { get; set; } = expires;
}

public class Login(ILogger<Login> logger)
{
    [Function("Login")]
    [OpenApiOperation(operationId: "GetWeather", tags: new[] { "Weather" })]
    [OpenApiParameter(name: "city", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "City name")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "Weather data")]
    public async Task<IActionResult> Run([Microsoft.Azure.Functions.Worker.HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        logger.LogInformation("Processing request for Login.");
        
        LoginUserObject? requestData;
        try
        {
            // Deserialize the request body
            requestData = JsonSerializer.Deserialize<LoginUserObject>(await new StreamReader(req.Body).ReadToEndAsync());
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

        var (success, token, expires) = await AuthenticationCore.LoginUser(requestData.Username, requestData.Password);
        
        if (!success)
        {
            return new OkObjectResult((token));
        }
        
        return new OkObjectResult(new BearerTokenObject(token, expires));
    }
}
