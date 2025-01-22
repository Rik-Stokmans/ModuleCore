using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;

namespace LogicLayer.Modules.LoggingModule.Interfaces;

public interface ILogService
{
    public Task CreateLog(LogMessage logObject);
    public Task<List<LogMessage>> GetLogs();
    public Task DeleteLogs();
}