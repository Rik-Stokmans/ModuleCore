using FunctionApi;
using LogicLayer.Core;
using LogicLayer.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MockDataLayer.Services;

//Init Core with Services
Core.Init(services =>
{
    services.Register<ILogService>(new LogMockService());
    // Add more services as needed
});

AzureFunctionGenerator.GenerateFunctions();
Console.WriteLine("Function generation completed.");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();