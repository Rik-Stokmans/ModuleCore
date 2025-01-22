using LogicLayer.Modules.LoggingModule.Models;

namespace Server.ModelViews;

public class LoggingView(DateTime time, string message)
{
    public DateTime Time { get; set; } = time;
    public string Message { get; set; } = message;
}

public static class LoggingViewExtensions
{
    public static LoggingView GetLogView(this LogMessage logMessageObject)
    {
        return new LoggingView(logMessageObject.Time, logMessageObject.Message);
    }
}