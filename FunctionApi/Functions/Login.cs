using System.Text.Json;
using AzureDatabase;
using LogicLayer;
using LogicLayer.Authentication;
using LogicLayer.Authentication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        logger.LogInformation("Processing request for Login.");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Optional: Set base path for configuration
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load from appsettings.json (if available)
            .AddEnvironmentVariables() // Correct method to load environment variables
            .Build();

        // Retrieve the connection string from environment variables (if set)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
            
        return new BadRequestObjectResult("string:" + connectionString);
        
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
            return new UnauthorizedResult();
        }
        
        return new OkObjectResult(new BearerTokenObject(token, expires));
    }
}