using System.Text.Json;
using LogicLayer;
using LogicLayer.Authentication;
using LogicLayer.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApi.Functions;


    public class LoginUserObject
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

public class Login(ILogger<Login> logger)
{
    [Function("Login")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
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

        var (success, token) = AuthenticationCore.LoginUser(requestData.Username, requestData.Password);
        
        if (!success)
        {
            return new UnauthorizedResult();
        }
        
        return new OkObjectResult(token);
    }
}