using AzureDatabase.Services;
using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockDataLayer.Services;

namespace FunctionApi;

public static class Program
{
    public static void Main(string[] args)
    {
        //Init Core with Services
        Core.Init(services =>
        {
            services.Register<ILogService>(new AzureLogService());
            services.Register<IAuthenticationService>(new AzureAuthenticationService());
            services.Register<IChildFocus>(new ChildFocusMockService());
            services.Register<INewsObjectService>(new NewsObjectTransientService());
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
    }
}