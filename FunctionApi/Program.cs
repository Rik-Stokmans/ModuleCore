using LogicLayer.Core;
using LogicLayer.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MockDataLayer.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

//Init Core with Services
Core.Init(services =>
{
    services.Register<ILogService>(new LogMockService());
    // Add more services as needed
});

host.Run();