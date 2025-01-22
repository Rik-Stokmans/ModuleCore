using AzureDatabase.Services;
using EntityFramework;
using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockDataLayer.Services;

namespace FunctionApi;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = FunctionsApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        
        // Retrieve the connection string from environment variables (if set)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<Context>(options =>
        {
            options.UseAzureSql(connectionString);
        });
        
        // Register services
        Core.Init(registry =>
        {
            // Register your services here
            registry.Register<ILogService>(new AzureLogService());
            registry.Register<IAuthenticationService>(new AzureAuthenticationService());
            registry.Register<IChildFocus>(new ChildFocusMockService());
            registry.Register<INewsObjectService>(new NewsObjectTransientService());
        });

        // Register services
        builder.Services.AddScoped<ILogService, AzureLogService>();
        builder.Services.AddScoped<IAuthenticationService, AzureAuthenticationService>();
        builder.Services.AddScoped<IChildFocus, ChildFocusMockService>();
        builder.Services.AddTransient<INewsObjectService, NewsObjectTransientService>();
        
        builder.Services.ConfigureFunctionsApplicationInsights();
        builder.Services.AddApplicationInsightsTelemetryWorkerService();
        
        var app = builder.Build();
        
        await app.RunAsync();
    }
}