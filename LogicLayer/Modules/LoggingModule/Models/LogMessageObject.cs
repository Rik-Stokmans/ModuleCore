namespace LogicLayer.Modules.LoggingModule.Models;

public class LogMessageObject(string message) : LogObjectBase
{
    public string Message { get; set; } = message;
}