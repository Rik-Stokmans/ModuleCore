using LogicLayer.Models;

namespace LogicLayer.Interfaces;

public interface ILogService
{
    [HttpMethod("POST")]
    public OperationResult CreateLog(LogMessageObject logObject);
    
    [HttpMethod("POST")]
    public OperationResult CreateLogTwice(LogMessageObject logObject);
    
    [HttpMethod("GET")]
    public (OperationResult, List<LogMessageObject>) GetLogs();
}