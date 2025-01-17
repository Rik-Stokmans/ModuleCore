using AzureDatabase.Services;
using LogicLayer;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using MockDataLayer.Services;

namespace FunctionApi
{
    public static class Startup
    {
        public static void startup()
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