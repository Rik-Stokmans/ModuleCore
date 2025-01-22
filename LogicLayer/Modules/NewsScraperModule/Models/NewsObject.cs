namespace LogicLayer.Modules.NewsScraperModule.Models;

public class NewsObject(DateTime time, string title)
{
    public string Title { get; set; } = title;
    public DateTime Time { get; set; } = time;
}