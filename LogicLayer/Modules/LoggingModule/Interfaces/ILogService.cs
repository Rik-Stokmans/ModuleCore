using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;

namespace LogicLayer.Modules.LoggingModule.Interfaces;

public interface ILogService
{
    public OperationResult CreateLog(LogMessageObject logObject);
    
    public OperationResult CreateLogTwice(LogMessageObject logObject);
    
    public (OperationResult, List<LogMessageObject>) GetLogs();
    public OperationResult DeleteLogs();
}