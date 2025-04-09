using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

namespace LogicLayer.Modules.LoggingModule.Models;

public class Feedback
{
    
    public Feedback(FeedbackConditions condition, string screenId, DateTime time, string comment = "")
    {
        Condition = condition;
        ScreenId = screenId;
        Time = time;
        Comment = comment;
    }

    public Feedback(FeedbackConditions condition, string screenId, string comment = "")
    {
        Condition = condition;
        ScreenId = screenId;
        Comment = comment;
    }
    
    [Key]
    public int Id { get; set; }
    public string ScreenId { get; set; }
    public FeedbackConditions Condition { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public string Comment { get; set; }
    
}