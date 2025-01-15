using LogicLayer.CoreModels;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Models;

namespace LogicLayer.Modules.NewsScraperModule;

public class NewsScraperModule
{
    [AuthPermissionClaim(PermissionClaim.Get)]
    [HttpMethod("GET")]
    public static (OperationResult, List<NuNlObject>) GetNews(string category)
    {
        var newsObjects = Core.GetService<INewsObjectService>().GetNewsObjects(category);
        
        return (OperationResult.GetSuccess(), newsObjects);
    }
}