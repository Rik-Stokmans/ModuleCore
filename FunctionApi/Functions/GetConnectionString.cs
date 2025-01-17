using System.Text.Json;
using LogicLayer.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunctionApi.Functions;

public class GetConnectionString(ILogger<GetConnectionString> logger)
{
    [Function("GetConnectionString")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        logger.LogInformation("Processing request for Login.");
        
        // Building the configuration and adding environment variables
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Optional: Set base path for configuration
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load from appsettings.json (if available)
            .AddEnvironmentVariables() // Correct method to load environment variables
            .Build();

        // Retrieve the connection string from environment variables (if set)
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        return new OkObjectResult(connectionString);
    }
}