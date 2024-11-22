using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace MockDataLayer.Services;

public class LogMockService : ILogService
{
    public Task<OperationResult> Log(LogMessageObject logMessageObject)
    {
        MockData.LogObjects.Add(logMessageObject);
        
        return Task.FromResult(OperationResult.GetCreated());
    }

    public Task<(OperationResult, List<LogMessageObject>)> GetAll()
    {
        return Task.FromResult((OperationResult.GetSuccess(), MockData.LogObjects));
    }
}