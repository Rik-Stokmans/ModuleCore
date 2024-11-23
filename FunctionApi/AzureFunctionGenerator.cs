using System.IO;
using LogicLayer.Core;

namespace FunctionApi;

public static class AzureFunctionGenerator
{
    public static void GenerateFunctions()
    {
        var methods = Core.GetHttpAnnotatedMethods();

        // Ensure output directory exists
        var outputDirectory = Path.Combine("bin", "Debug", "net8.0", "Generated");
        if (!Directory.Exists("Generated"))
        {
            Directory.CreateDirectory("Generated");
        }

        foreach (var (serviceType, method, verb) in methods)
        {
            var functionName = $"{serviceType.Name}_{method.Name}";
            var functionClassName = $"{serviceType.Name}Functions";

            // Generate the function code
            var functionCode = $$"""
            using LogicLayer.Core;
            using LogicLayer.Interfaces;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Mvc;
            using Microsoft.Azure.Functions.Worker;
            using Microsoft.Extensions.Logging;

            namespace FunctionApi.bin.Debug.net8._0.Generated;

            public class {{functionClassName}}(ILogger<{{functionClassName}}> logger)
            {
                [Function("{{functionName}}")]
                public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "{{verb.ToLowerInvariant()}}")] HttpRequest req)
                {
                    logger.LogInformation("C# HTTP trigger function processed a request.");

                    var logMessage = req.Query["message"];

                    if (string.IsNullOrEmpty(logMessage))
                    {
                        return new BadRequestObjectResult("Please pass a message on the query string using ?message=<message>");
                    }

                    var result = Core.GetService<{{serviceType.Name}}>().{{method.Name}}(logMessage);

                    return (result.Code / 100) switch
                    {
                        2 => new OkObjectResult(result),
                        4 => new BadRequestObjectResult(result),
                        _ => new StatusCodeResult(result.Code)
                    };
                }
            }
            """;

            // Save the generated file
            File.WriteAllText($"Generated/{functionClassName}_{method.Name}.cs", functionCode);

        }
    }
}
