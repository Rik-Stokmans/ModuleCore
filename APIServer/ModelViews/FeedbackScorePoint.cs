namespace Server.ModelViews;

public class FeedbackScorePoint
{
    public DateTime Timestamp { get; set; }
    public double Score { get; set; }
    public string? Comment { get; set; }
}