using LogicLayer.Modules.NewsScraperModule.Models;

namespace LogicLayer.Modules.NewsScraperModule.Interfaces;

public interface INewsObjectService
{
    public List<NuNlObject> GetNewsObjects(string category);
}