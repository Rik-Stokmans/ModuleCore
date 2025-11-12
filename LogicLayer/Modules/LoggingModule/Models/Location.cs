using System.ComponentModel.DataAnnotations;

namespace LogicLayer.Modules.LoggingModule.Models;

public class Location(string screenId)
{
    public Location() : this("")
    {
    }

    [Key]
    public string ScreenId { get; set; } = screenId;
    public string Name { get; set; } = "No Name";
}