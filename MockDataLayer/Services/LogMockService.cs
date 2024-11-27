using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace MockDataLayer.Services;

public class LogMockService : ILogService
{
    public OperationResult CreateLog(LogMessageObject logObject)
    {
        Console.WriteLine($"Log: {logObject.Message} at {logObject.Time}");
        
        MockData.LogObjects.Add(logObject);
        
        return OperationResult.GetSuccess();
    }

    public OperationResult CreateLogTwice(LogMessageObject logObject)
    {
        var logObject1 = new LogMessageObject(logObject.Message);
        Console.WriteLine($"Log: {logObject1.Message} at {logObject1.Time}");
        
        var logObject2 = new LogMessageObject(logObject.Message);
        Console.WriteLine($"Log: {logObject2.Message} at {logObject2.Time}");
        
        MockData.LogObjects.Add(logObject1);
        MockData.LogObjects.Add(logObject2);
        
        return OperationResult.GetSuccess();
    }

    public (OperationResult, List<LogMessageObject>) GetLogs()
    {
        return (OperationResult.GetSuccess(), MockData.LogObjects);
    }
}
