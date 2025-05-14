namespace Server.ModelViews;

public class FeedbackDetailsView
{
    public string ScreenId { get; set; }
    public DateTime LastActivity { get; set; }
    public List<FeedbackScorePoint> Feedback { get; set; }
}