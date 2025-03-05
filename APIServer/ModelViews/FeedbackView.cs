using LogicLayer.Modules.LoggingModule.Models;

namespace Server.ModelViews;

public class FeedbackView(DateTime time, string screenId, FeedbackConditions condition)
{
    public DateTime Time { get; set; } = time;
    public string ScreenId { get; set; } = screenId;
    public FeedbackConditions Condition { get; set; } = condition;
}

public static class FeedbackViewExtensions
{
    public static FeedbackView GetFeedbackView(this Feedback feedbackObject)
    {
        return new FeedbackView(feedbackObject.Time, feedbackObject.ScreenId, feedbackObject.Condition);
    }
}