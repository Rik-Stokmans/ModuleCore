using LogicLayer.Modules.ModuleCollectionBuilder.Models;

namespace LogicLayer;

public interface IModule
{
    public ModuleHtmlObject GetModuleHtml();
}