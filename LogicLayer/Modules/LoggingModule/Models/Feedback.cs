using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LogicLayer.Modules.LoggingModule.Models;

public class Feedback
{
    
    public Feedback(FeedbackConditions condition, string screenId, DateTime time)
    {
        Condition = condition;
        ScreenId = screenId;
        Time = time;
    }

    public Feedback(FeedbackConditions condition, string screenId)
    {
        Condition = condition;
        ScreenId = screenId;
    }
    
    [Key]
    public int Id { get; set; }
    public string ScreenId { get; set; }
    public FeedbackConditions Condition { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
    
}