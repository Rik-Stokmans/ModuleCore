namespace LogicLayer.Modules.NewsScraperModule.Models;

public class NuNlObject(DateTime dateTime, string title)
{
    public DateTime DateTime { get; set; } = dateTime;
    public string Title { get; set; } = title;
}