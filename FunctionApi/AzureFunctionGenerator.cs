using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using LogicLayer.Core;

namespace FunctionApi
{
    public static class AzureFunctionGenerator
    {
        private static string GenerateValidationCode(string paramName, Type paramType)
        {
            if (paramType.IsValueType && Nullable.GetUnderlyingType(paramType) == null) // Non-nullable value type
            {
                return $@"
                            if (requestData.{paramName} == default)
                            {{
                                errors.Add(""Parameter '{paramName}' is required but was not provided."");
                            }}";
            }

            return $@"
                            if (requestData.{paramName} == null)
                            {{
                                errors.Add(""Parameter '{paramName}' is required but was null."");
                            }}";
        }
        
        public static void GenerateFunctions()
        {
            var methods = Core.GetHttpAnnotatedMethods();

            // Ensure output directory exists
            const string outputDirectory = "Generated";
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var (serviceType, method, verb) in methods)
            {
                // Class and DTO Names
                var functionName = serviceType.Name.StartsWith("I")
                    ? serviceType.Name.Substring(1)
                    : serviceType.Name;

                var classname = $"{functionName}{method.Name}";
                var dtoClassName = $"{classname}Request";

                // Generate DTO Class
                var dtoProperties = method.GetParameters()
                    .Select(p => $"public {p.ParameterType.Name} {p.Name} {{ get; set; }}")
                    .ToList();

                var dtoClassCode = @$"
                public class {dtoClassName}
                {{
                    {string.Join(Environment.NewLine, dtoProperties)}
                }}
                ";

                // Generate the Azure Function
                var functionCode = @$"
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Linq;
                using System.Text.Json;
                using System.Threading.Tasks;
                using LogicLayer.Core;
                using LogicLayer.Models;
                using LogicLayer.Interfaces;
                using Microsoft.AspNetCore.Http;
                using Microsoft.AspNetCore.Mvc;
                using Microsoft.Azure.Functions.Worker;
                using Microsoft.Extensions.Logging;

                namespace FunctionApi.Generated
                {{
                    {dtoClassCode}

                    public class {classname}
                    {{
                        private readonly ILogger<{classname}> _logger;

                        public {classname}(ILogger<{classname}> logger)
                        {{
                            _logger = logger;
                        }}

                        [Function(""{method.Name}"")]
                        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, ""{verb.ToLowerInvariant()}"")] HttpRequest req)
                        {{
                            _logger.LogInformation(""Processing request for {method.Name}."");

                            {dtoClassName}? requestData;
                            try
                            {{
                                // Deserialize the request body
                                requestData = JsonSerializer.Deserialize<{dtoClassName}>(await new StreamReader(req.Body).ReadToEndAsync());
                                if (requestData == null)
                                {{
                                    return new BadRequestObjectResult(""Request body is null or empty."");
                                }}
                            }}
                            catch (Exception ex)
                            {{
                                Console.WriteLine(req.Body.ToString());
                                _logger.LogError(ex, ""Failed to parse request body."");
                                return new BadRequestObjectResult(""Invalid JSON in request body."");
                            }}

                            // Validate parameters
                            var errors = new List<string>();
                            {string.Join(Environment.NewLine, method.GetParameters().Select<ParameterInfo, string>(p => GenerateValidationCode(p.Name!, p.ParameterType)))}

                            if (errors.Any())
                            {{
                                return new BadRequestObjectResult(string.Join(""; "", errors));
                            }}

                            // Call the service method
                            var result = Core.GetService<{serviceType.Name}>().{method.Name}(
                                {string.Join(", ", method.GetParameters().Select(p => $"requestData.{p.Name}"))}
                            );

                            // Return the result
                            return (result.Code / 100) switch
                            {{
                                2 => new OkObjectResult(result),
                                4 => new BadRequestObjectResult(result),
                                _ => new StatusCodeResult(result.Code)
                            }};
                        }}
                    }}
                }}
                ";

                // Write to a file
                File.WriteAllText($"Generated/{classname}.cs", functionCode);
            }
        }
    }
}
