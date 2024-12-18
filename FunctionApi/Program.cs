using AzureDatabase.Services;
using FunctionApi;
using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MockDataLayer.Services;

//Init Core with Services
Core.Init(services =>
{
    services.Register<ILogService>(new AzureLogService());
    services.Register<IAuthenticationService>(new AzureAuthenticationService());
    services.Register<IChildFocus>(new ChildFocusMockService());
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