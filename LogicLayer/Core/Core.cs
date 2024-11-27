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

        public static IEnumerable<(Type DeclaringType, MethodInfo Method, string HttpVerb)> GetHttpAnnotatedMethods()
        {
            CheckInit();

            // Log the start of the method discovery process
            Console.WriteLine("Starting to scan Core class for methods annotated with HttpMethodAttribute.");

            // Scan for methods within the Core class that are annotated with HttpMethodAttribute
            var coreType = typeof(Core);

            foreach (var method in coreType.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                // Log each method being inspected
                Console.WriteLine($"Inspecting method: {method.Name}");

                var attribute = method.GetCustomAttribute<HttpMethodAttribute>();
                if (attribute != null)
                {
                    // Log when an annotated method is discovered
                    Console.WriteLine($"Discovered annotated method: {method.Name}, HTTP Verb: {attribute.Verb}");

                    yield return (coreType, method, attribute.Verb);
                }
            }

            // Log the end of the discovery process
            Console.WriteLine("Finished scanning Core class for annotated methods.");
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
