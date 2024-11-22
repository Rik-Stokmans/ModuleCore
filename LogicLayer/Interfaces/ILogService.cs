using LogicLayer.Models;

namespace LogicLayer.Interfaces;

public interface ILogService
{
    public Task<OperationResult> Log(LogMessageObject logMessageObject);
    
    public Task<(OperationResult, List<LogMessageObject>)> GetAll();
}