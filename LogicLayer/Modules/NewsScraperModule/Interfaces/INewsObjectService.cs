using LogicLayer.Modules.NewsScraperModule.Models;

namespace LogicLayer.Modules.NewsScraperModule.Interfaces;

public interface INewsObjectService
{
    public Task<Dictionary<Country, List<NewsObject>>> GetNewsObjects();
}