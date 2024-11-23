using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogicLayer.Models;

namespace LogicLayer.Core
{
    public static partial class Core
    {
        private static readonly ConcurrentDictionary<Type, object> Services = new();
        private static bool _initialized;

        public static void Init(Action<ServiceRegistry> configureServices)
        {
            var registry = new ServiceRegistry();
            configureServices(registry);

            foreach (var (type, instance) in registry.RegisteredServices)
            {
                Services[type] = instance;
            }

            _initialized = true;
        }

        public static T GetService<T>() where T : class
        {
            CheckInit();

            if (Services.TryGetValue(typeof(T), out var service) && service is T typedService)
            {
                return typedService;
            }

            throw new Exception($"Service of type {typeof(T).Name} not registered");
        }

        public static IEnumerable<(Type ServiceType, MethodInfo Method, string HttpVerb)> GetHttpAnnotatedMethods()
        {
            CheckInit();

            foreach (var serviceType in Services.Keys)
            {
                Console.WriteLine($"Checking Service Type: {serviceType.Name}");

                foreach (var method in serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    Console.WriteLine($"Checking Method: {method.Name}");
                    var attribute = method.GetCustomAttribute<HttpMethodAttribute>();
                    
                    
                    
                    
                    if (attribute != null)
                    {
                        Console.WriteLine($"Discovered Method: {method.Name} with verb: {attribute.Verb}");
                        yield return (serviceType, method, attribute.Verb);
                    }
                }
            }
        }



        private static void CheckInit()
        {
            if (!_initialized) throw new Exception("Core not initialized");
        }
    }

    public class ServiceRegistry
    {
        internal readonly ConcurrentDictionary<Type, object> RegisteredServices = new();

        public void Register<T>(T service) where T : class
        {
            ArgumentNullException.ThrowIfNull(service);
            RegisteredServices[typeof(T)] = service;
        }
    }
}
