namespace LogicLayer.Modules.LoggingModule.Models;

public class LogMessageObject : LogObjectBase
{
    public LogMessageObject(string message)
    {
        Message = message;
    }
    
    public LogMessageObject(string message, DateTime time)
    {
        Message = message;
        Time = time;
    }

    public LogMessageObject()
    {
    }

    public string Message { get; set; }
}