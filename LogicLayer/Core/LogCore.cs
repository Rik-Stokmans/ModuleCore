using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace LogicLayer.Core;

public static partial class Core
{
    [HttpMethod("POST")]
    public static OperationResult CreateLog(LogMessageObject logObject)
    {
        CheckInit();
        
        var result =  GetService<ILogService>().CreateLog(logObject);
        
        return result;
    }
    
    [HttpMethod("POST")]
    public static OperationResult CreateLogTwice(LogMessageObject logObject)
    {
        CheckInit();
        
        var result = GetService<ILogService>().CreateLogTwice(logObject);

        return result;
    }

    [HttpMethod("GET")]
    public static (OperationResult, List<LogMessageObject>) GetLogs()
    {
        CheckInit();
        
        var (result, data) = GetService<ILogService>().GetLogs();
        
        return (result, data);
    }
}