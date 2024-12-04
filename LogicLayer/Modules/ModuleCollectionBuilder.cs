using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace LogicLayer.Modules
{
    public class ModuleCollectionBuilder
    {
        [HttpMethod("GET")]
        public static (OperationResult, List<ModuleHtmlObject>) GetAllModuleHtml()
        {
            List<IModule> modules = [];
            // Log the start of the discovery process
            Console.WriteLine("Starting to scan LogicLayer.Modules namespace for classes implementing IModule.");

            // Get all assemblies currently loaded in the app domain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types in the assembly
                foreach (var type in assembly.GetTypes())
                {
                    // Check if the type belongs to the LogicLayer.Modules namespace
                    if (type.Namespace != "LogicLayer.Modules") continue;
                    // Check if the type implements the IModule interface
                    if (!typeof(IModule).IsAssignableFrom(type)) continue;
                    // Add the type to the list of modules
                    if ((IModule)Activator.CreateInstance(type) != null)
                    {
                        modules.Add((IModule)Activator.CreateInstance(type));
                    }
                }
            }
            
            Console.WriteLine("Finished scanning for IModule implementations.");
            
            List<ModuleHtmlObject> moduleHtmlObjects = [];
            moduleHtmlObjects.AddRange(modules.Select(module => new ModuleHtmlObject(module.GetModuleHtml())));
            
            return (OperationResult.GetSuccess(), moduleHtmlObjects);
        }
    }
}