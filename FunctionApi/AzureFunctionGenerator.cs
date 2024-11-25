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
            var functionName = serviceType.Name.StartsWith("I") 
                ? string.Concat(serviceType.Name.AsSpan(1), "")
                : serviceType.Name;

            var classname = functionName + method.Name;

            //TODO make the code within the function conform to the method
            // Generate the function code
            var functionCode = $$"""
            using LogicLayer.Core;
            using LogicLayer.Interfaces;
            using Microsoft.AspNetCore.Http;
            using Microsoft.AspNetCore.Mvc;
            using Microsoft.Azure.Functions.Worker;
            using Microsoft.Extensions.Logging;

            namespace FunctionApi.bin.Debug.net8._0.Generated;

            public class {{classname}}(ILogger<{{classname}}> logger)
            {
                [Function("{{method.Name}}")]
                public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "{{verb.ToLowerInvariant()}}")] HttpRequest req)
                {
                    logger.LogInformation("C# HTTP trigger function processed a request.");

                    

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
            File.WriteAllText($"Generated/{classname}.cs", functionCode);

        }
    }
}
