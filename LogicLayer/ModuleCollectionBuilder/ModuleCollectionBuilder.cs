using LogicLayer.CoreModels;
using LogicLayer.ModuleCollectionBuilder.Models;

namespace LogicLayer.ModuleCollectionBuilder
{
    public static class ModuleCollectionBuilder
    {
        [HttpMethod("GET")]
        public static (OperationResult, List<ModuleHtmlObject>) GetAllModuleHtml()
        {
            List<IModule> modules = [];
            
            // Get all assemblies currently loaded in the app domain
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types in the assembly
                foreach (var type in assembly.GetTypes())
                {
                    
                    // Check if the type belongs to the LogicLayer.Modules namespace
                    if (type.Namespace != null && !type.Namespace.StartsWith("LogicLayer.Modules") ) continue;
                    // Check if the type implements the IModule interface
                    if (!typeof(IModule).IsAssignableFrom(type)) continue;
                    // Add the type to the list of modules
                    try
                    {
                        if ((IModule)Activator.CreateInstance(type) != null)
                        {
                            modules.Add((IModule)Activator.CreateInstance(type));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error creating instance of module:");
                        Console.WriteLine(e);
                    }
                }
            }
            
            List<ModuleHtmlObject> moduleHtmlObjects = [];
            moduleHtmlObjects.AddRange(modules.Select(module => new ModuleHtmlObject(module.GetModuleHtml())));
            
            return (OperationResult.GetSuccess(), moduleHtmlObjects);
        }
    }
}