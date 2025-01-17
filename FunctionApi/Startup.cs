using AzureDatabase.Services;
using FunctionApi;
using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using MockDataLayer.Services;

[assembly: FunctionsStartup(typeof(Startup))]

namespace FunctionApi
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Core.Init(registry =>
            {
                // Register your services here
                registry.Register<ILogService>(new AzureLogService());
                registry.Register<IAuthenticationService>(new AzureAuthenticationService());
                registry.Register<IChildFocus>(new ChildFocusMockService());
                registry.Register<INewsObjectService>(new NewsObjectTransientService());
            });
        }
    }
}