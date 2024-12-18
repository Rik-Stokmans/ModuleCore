using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;

namespace LogicLayer.Modules.LoggingModule.Interfaces;

public interface ILogService
{
    public Task<OperationResult> CreateLog(LogMessageObject logObject);
    public Task<(OperationResult, List<LogMessageObject>)> GetLogs();
    public Task<OperationResult> DeleteLogs();
}