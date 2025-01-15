using System.Reflection;
using LogicLayer;
using LogicLayer.CoreModels;

namespace FunctionApi
{
    public static class AzureFunctionGenerator
    {
        private static string GenerateValidationCode(string paramName, Type paramType)
        {
            if (paramType.IsValueType && Nullable.GetUnderlyingType(paramType) == null) // Non-nullable value type
            {
                return $$"""
                                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                                    if (requestData.{{paramName}} == default)
                                    {
                                        errors.Add("Parameter '{{paramName}}' is required but was not provided.");
                                    }
                         """;
            }

            return $$"""
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                                if (requestData.{{paramName}} == null)
                                {
                                    errors.Add("Parameter '{{paramName}}' is required but was null.");
                                }
                     """;
        }
        
        public static void GenerateFunctions()
        {
            var methods = Core.GetHttpAnnotatedMethods();

            var valueTuples = methods as (Type DeclaringType, MethodInfo Method, string HttpVerb)[] ?? methods.ToArray();

            // Ensure output directory exists
            const string outputDirectory = "Generated";
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var (serviceType, method, verb) in valueTuples)
            {
                var classType = serviceType.Name;
                
                var classname = $"{method.Name}Function";
                var dtoClassName = $"{classname}ParameterObject";

                // Generate DTO Class
                var parameters = method.GetParameters();
                var hasParameters = parameters.Any();
                var dtoProperties = parameters
                    .Select(p => $"public {p.ParameterType.Name} {char.ToUpper(p.Name[0]) + p.Name.Substring(1)} {{ get; set; }}")

                    .ToList();

                var dtoClassCode = hasParameters ? $$"""
                                     
                                                                 public class {{dtoClassName}}
                                                                 {
                                                                     {{string.Join(Environment.NewLine, dtoProperties)}}
                                                                 }
                                                     
                                                            """ : string.Empty;

                // Determine how to handle the return type
                string returnType = method.ReturnType.Name;
                string resultHandlingCode;

                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
                {
                    // Handle tuple return type (e.g., (OperationResult, List<T>))
                    var genericArgs = method.ReturnType.GetGenericArguments();
                    string operationResultType = genericArgs[0].Name;
                    string dataType = genericArgs[1].Name;

                    resultHandlingCode = $$"""
                                           
                                                        var (result, data) = {{classType}}.{{method.Name}}(
                                                            {{string.Join(", ", parameters.Select(p => $"requestData.{char.ToUpper(p.Name[0]) + p.Name.Substring(1)}"))}}
                                                        );
                               
                                                        return result.Code switch
                                                        {
                                                            >= 200 and < 300 => new OkObjectResult(data),
                                                            >= 400 and < 500 => new BadRequestObjectResult(result),
                                                            _ => new StatusCodeResult(result.Code)
                                                        };
                                                               
                                           """;
                }
                else
                {
                    // Handle non-tuple return type (e.g., a single object or primitive value)
                    resultHandlingCode =  $$"""
                                            
                                                        var result = {{classType}}.{{method.Name}}(
                                                            #pragma warning disable CS8604 // Possible null reference argument.
                                                            {{string.Join(", ", parameters.Select(p => $"requestData.{char.ToUpper(p.Name[0]) + p.Name.Substring(1)}"))}}
                                                            #pragma warning restore CS8604 // Possible null reference argument.
                                                        );
                                
                                                        return (result.Code / 100) switch
                                                        {
                                                            2 => new OkObjectResult(result),
                                                            4 => new BadRequestObjectResult(result),
                                                            _ => new StatusCodeResult(result.Code)
                                                        };
                                                                
                                            """;
                }

                // Generate the Azure Function
                var functionCode = $$"""
                                     using System.Reflection;
                                     using System.Text.Json;
                                     using LogicLayer.CoreModels;
                                     using LogicLayer.Modules.{{classType}};
                                     using LogicLayer.Modules.{{classType}}.Models;
                                     using Microsoft.AspNetCore.Http;
                                     using Microsoft.AspNetCore.Mvc;
                                     using Microsoft.Azure.Functions.Worker;
                                     using Microsoft.Extensions.Logging;
                                     using static Authenticator.Authenticator;
                     
                                     namespace FunctionApi.Generated
                                     {
                                         {{dtoClassCode}}
                     
                                         public class {{classname}}(ILogger<{{classname}}> logger)
                                         {
                                             [Function("{{method.Name}}")]
                                             public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "{{verb.ToLowerInvariant()}}")] HttpRequest req)
                                             {
                                                 logger.LogInformation("Processing request for {{method.Name}}.");
                     
                                                 // Authenticate the request
                                                 var isAuthenticated = true;
                                                 
                                                 var (permissions, client) = GetAuthenticationPermissions(req.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()));
                                                 
                                                 try
                                                 {
                                                     (typeof({{classType}}).GetMethod("{{method.Name}}") ?? throw new InvalidOperationException()).GetCustomAttributes(typeof(AuthPermissionClaim)).ToList().ForEach(claim =>
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
                                     
                                     
                                                    
                                                 {{(hasParameters ? 
                                                     $$"""
                                                        {{dtoClassName}}? requestData;
                                                                    try
                                                                    {
                                                                        // Deserialize the request body
                                                                        requestData = JsonSerializer.Deserialize<{{dtoClassName}}>(await new StreamReader(req.Body).ReadToEndAsync());
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
                                                        {{string.Join(Environment.NewLine, parameters.Select(p => GenerateValidationCode(char.ToUpper(p.Name[0]) + p.Name.Substring(1), p.ParameterType)))}}

                                                                    if (errors.Any())
                                                                    {
                                                                        return new BadRequestObjectResult(string.Join("; ", errors));
                                                                    }
                                                        """ : string.Empty)}}
                     
                                                 // Call the service method and handle the result
                                                 {{resultHandlingCode}}
                                             }
                                         }
                                     }
                                                     
                                     """;

                // Write to a file
                //File.WriteAllText($"Generated/{classname}.cs", functionCode);
                
                var projectDir = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                
                Console.WriteLine(projectDir);
                
                if (projectDir == null) continue;
                var generatedPath = Path.Combine(projectDir, "Generated");
                Directory.CreateDirectory(generatedPath); // Ensure the directory exists
                File.WriteAllText(Path.Combine(generatedPath, $"{classname}.cs"), functionCode);
            }
        }
    }
}
