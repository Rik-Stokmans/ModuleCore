using System.Xml.Linq;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using LogicLayer.Modules.NewsScraperModule.Models;

namespace MockDataLayer.Services;

public class NewsObjectTransientService : INewsObjectService
{
    private static Dictionary<Country, string> NewsEndPoints { get; set; } = new()
    {
        { Country.Netherlands, "https://www.nu.nl/rss/Algemeen" },
        { Country.Germany, "https://www.tagesschau.de/infoservices/alle-meldungen-100~rss2.xml" },
        { Country.Latvia, "https://www.apollo.lv/rss" },
        { Country.Romania, "http://rss.realitatea.net/homepage.xml" }
    };

    public async Task<Dictionary<Country, List<NewsObject>>> GetNewsObjects()
    {
        // Initialize the dictionary to store fetched news
        var fetchedNewsObjects = new Dictionary<Country, List<NewsObject>>();

        foreach (var (country, endpoint) in NewsEndPoints)
        {
            try
            {
                // Fetch news objects for the current endpoint
                var newsObjects = await TryFetchNewsObjects(endpoint);
                
                if (newsObjects.Count == 0) continue;
            
                // Add the news objects to the dictionary
                fetchedNewsObjects[country] = newsObjects;
            }
            catch (Exception ex)
            {
                // Handle or log the error gracefully
                Console.WriteLine($"Error fetching news for {country}: {ex.Message}");
            }
        }

        return fetchedNewsObjects;
    }


    private async Task<List<NewsObject>> TryFetchNewsObjects(string endpoint)
    {
        var newsObjects = new List<NewsObject>();

        try
        {
            // Simulating fetching RSS data (use HttpClient in a real scenario)
            var rssFeed = await FetchRssFeed(endpoint);

            // Parse the RSS feed and convert it into NewsObject instances
            newsObjects = ParseRssToNewsObjects(rssFeed);
        }
        catch (Exception ex)
        {
            // Log or handle exceptions
            Console.WriteLine($"Failed to fetch or parse news from {endpoint}: {ex.Message}");
        }

        return newsObjects;
    }
    
    private Task<string> FetchRssFeed(string endpoint)
    {
        using var client = new HttpClient();
        var response = client.GetStringAsync(endpoint).Result;
        return Task.FromResult(response);
    }

    private List<NewsObject> ParseRssToNewsObjects(string rssData)
    {
        var newsObjects = new List<NewsObject>();

        try
        {
            // Load the RSS data into an XDocument
            var xmlDoc = XDocument.Parse(rssData);

            // Traverse the XML structure to extract the "item" elements
            var items = xmlDoc.Descendants("item");

            foreach (var item in items)
            {
                // Extract relevant fields from each "item" element
                var title = item.Element("title")?.Value;
                var pubDate = item.Element("pubDate")?.Value;

                // Convert the pubDate to a DateTime object if it's not null
                DateTime.TryParse(pubDate, out var parsedDate);
                
                if (title == null || parsedDate == DateTime.MinValue) continue;

                // Create a NewsObject and populate its fields
                var newsObject = new NewsObject(parsedDate, title);

                // Add the newsObject to the list
                newsObjects.Add(newsObject);
            }
        }
        catch (Exception ex)
        {
            // Handle or log any errors that occur during parsing
            Console.WriteLine($"Error parsing RSS data: {ex.Message}");
        }

        return newsObjects;
    }



}






















