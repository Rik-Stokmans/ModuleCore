using LogicLayer.Models;

namespace LogicLayer.Interfaces;

public interface ILogService
{
    public OperationResult CreateLog(LogMessageObject logObject);
    
    public OperationResult CreateLogTwice(LogMessageObject logObject);
    
    public (OperationResult, List<LogMessageObject>) GetLogs();
}