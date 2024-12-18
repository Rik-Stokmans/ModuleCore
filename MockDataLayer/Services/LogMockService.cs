using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Models;

namespace MockDataLayer.Services;

public class LogMockService : ILogService
{
    public Task<OperationResult> CreateLog(LogMessageObject logObject)
    {
        Console.WriteLine($"Log: {logObject.Message} at {logObject.Time}");
        
        MockData.LogObjects.Add(logObject);
        
        return Task.FromResult(OperationResult.GetSuccess());
    }

    public Task<(OperationResult, List<LogMessageObject>)> GetLogs()
    {
        return Task.FromResult((OperationResult.GetSuccess(), MockData.LogObjects));
    }
    
    public Task<OperationResult> DeleteLogs()
    {
        MockData.LogObjects.Clear();
        
        return Task.FromResult(OperationResult.GetSuccess());
    }
}
