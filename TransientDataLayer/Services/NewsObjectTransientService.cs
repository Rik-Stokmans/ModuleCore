using LogicLayer.Modules.NewsScraperModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Models;

namespace MockDataLayer.Services;

public class NewsObjectTransientService : INewsObjectService
{
    public Dictionary<string, (DateTime, List<NuNlObject>)> newsObjects;
    
    public List<string> Categories { get; set; } = ["algemeen", "binnenland", "buitenland", "economie", "sport"];


    public List<NuNlObject> GetNewsObjects(string category)
    {
        if (!Categories.Contains(category)) return [];

        if (newsObjects.ContainsKey(category))
        {
            var (lastFetched, newsObjectsList) = newsObjects[category];

            if (lastFetched.AddHours(1) > DateTime.Now)
            {
                return newsObjectsList;
            }
        }

        var fetchedNewsObjects = TryFetchNewsObjects(category);

        if (fetchedNewsObjects.Count > 0)
        {
            newsObjects.Add(category, (DateTime.Now, fetchedNewsObjects));
            return fetchedNewsObjects;
        }

        return new List<NuNlObject>();
    }

    private List<NuNlObject> TryFetchNewsObjects(string category)
    {
        throw new NotImplementedException();
    }
}