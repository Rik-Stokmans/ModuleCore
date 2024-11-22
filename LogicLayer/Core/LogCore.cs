using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace LogicLayer.Core;

public static partial class Core
{
    
    public static async Task<OperationResult> CreateLog(LogMessageObject logMessageEntry)
    {
        CheckInit();
        
        var result = await GetService<ILogService>().Log(logMessageEntry);
        
        return result;
    }

    public static async Task<(OperationResult, List<LogMessageObject>)> GetAllLogs()
    {
        CheckInit();
        
        var result = await GetService<ILogService>().GetAll();
        
        return result;
    }
}