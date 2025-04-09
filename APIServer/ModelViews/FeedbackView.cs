using LogicLayer.Modules.LoggingModule.Models;

namespace Server.ModelViews;

public class FeedbackView(DateTime time, string screenId, FeedbackConditions condition, string comment)
{
    public DateTime Time { get; set; } = time;
    public string ScreenId { get; set; } = screenId;
    public FeedbackConditions Condition { get; set; } = condition;

    public string Comment { get; set; } = comment;
}

public static class FeedbackViewExtensions
{
    public static FeedbackView GetFeedbackView(this Feedback feedbackObject)
    {
        return new FeedbackView(feedbackObject.Time, feedbackObject.ScreenId, feedbackObject.Condition, feedbackObject.Comment);
    }
    
    public static Feedback Convert(this FeedbackView feedbackView)
    {
        return new Feedback(feedbackView.Condition, feedbackView.ScreenId, feedbackView.Time, feedbackView.Comment);
    }
    
    public static List<FeedbackView> Convert(this List<Feedback> feedbacks)
    {
        return feedbacks.Select(feedback => feedback.GetFeedbackView()).ToList();
    }
}