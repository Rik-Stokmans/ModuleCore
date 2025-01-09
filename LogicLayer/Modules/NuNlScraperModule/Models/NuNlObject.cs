namespace LogicLayer.Modules.NuNlScraperModule.Models;

public class NuNlObject(DateTime dateTime, string title)
{
    public DateTime DateTime { get; set; } = dateTime;
    public string Title { get; set; } = title;
}