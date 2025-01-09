using LogicLayer.CoreModels;
using LogicLayer.Modules.NuNlScraperModule.Interfaces;
using LogicLayer.Modules.NuNlScraperModule.Models;

namespace LogicLayer.Modules.NuNlScraperModule;

public class NuNlScraperModule
{
    [AuthPermissionClaim(PermissionClaim.Get)]
    [HttpMethod("GET")]
    public static (OperationResult, List<NuNlObject>) GetNews(string category)
    {
        var newsObjects = Core.GetService<INewsObjectService>().GetNewsObjects(category);
        
        return (OperationResult.GetSuccess(), newsObjects);
    }
}