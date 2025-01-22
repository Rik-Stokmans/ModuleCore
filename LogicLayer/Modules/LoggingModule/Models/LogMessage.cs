using System.ComponentModel.DataAnnotations;

namespace LogicLayer.Modules.LoggingModule.Models;

public class LogMessage
{
    public LogMessage(string message, DateTime time)
    {
        Message = message;
        Time = time;
    }

    public LogMessage(string message)
    {
        Message = message;
    }
    
    public LogMessage()
    {
    }

    [Key]
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
}