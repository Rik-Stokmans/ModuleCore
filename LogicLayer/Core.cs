using System.Collections.Concurrent;
using System.Reflection;
using LogicLayer.CoreModels;

namespace LogicLayer
{
    public static class Core
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

            // Log the start of the discovery process
            Console.WriteLine("Starting to scan LogicLayer.Modules namespace for methods annotated with HttpMethodAttribute.");

            // Get all assemblies currently loaded in the app domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                // Get all types in the assembly
                foreach (var type in assembly.GetTypes())
                {
                    // Check if the type belongs to the LogicLayer.Modules namespace
                    if (type.Namespace == "LogicLayer.Modules")
                    {
                        // Log the type being inspected
                        Console.WriteLine($"Inspecting type: {type.FullName}");

                        // Get public static methods of the type
                        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        {
                            // Check if the method has the HttpMethodAttribute
                            var attribute = method.GetCustomAttribute<HttpMethodAttribute>();
                            if (attribute != null)
                            {
                                // Log when an annotated method is discovered
                                Console.WriteLine($"Discovered annotated method: {method.Name} in type {type.Name}, HTTP Verb: {attribute.Verb}");

                                yield return (type, method, attribute.Verb);
                            }
                        }
                    }
                }
            }

            // Log the end of the discovery process
            Console.WriteLine("Finished scanning LogicLayer.Modules namespace for annotated methods.");
        }




        public static void CheckInit()
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
