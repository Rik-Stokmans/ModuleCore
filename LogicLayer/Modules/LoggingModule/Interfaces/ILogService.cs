using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;

namespace LogicLayer.Modules.LoggingModule.Interfaces;

public interface ILogService
{
    public Task<OperationResult> CreateLog(LogMessageObject logObject, int retryCount = 0);
    public Task<(OperationResult, List<LogMessageObject>)> GetLogs(int retryCount = 0);
    public Task<OperationResult> DeleteLogs(int retryCount = 0);
}