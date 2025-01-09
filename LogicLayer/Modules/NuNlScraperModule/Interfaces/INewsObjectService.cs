using LogicLayer.Modules.NuNlScraperModule.Models;

namespace LogicLayer.Modules.NuNlScraperModule.Interfaces;

public interface INewsObjectService
{
    public List<NuNlObject> GetNewsObjects(string category);
}