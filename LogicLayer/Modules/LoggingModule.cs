using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace LogicLayer.Modules;

public class LoggingModule : IModule
{
    [HttpMethod("POST")]
    public static OperationResult CreateLog(LogMessageObject logObject)
    {
        Core.Core.CheckInit();
        
        var result = Core.Core.GetService<ILogService>().CreateLog(logObject);
        
        return result;
    }
    
    [HttpMethod("POST")]
    public static OperationResult CreateLogTwice(LogMessageObject logObject)
    {
        Core.Core.CheckInit();
        
        var result = Core.Core.GetService<ILogService>().CreateLogTwice(logObject);

        return result;
    }

    [HttpMethod("GET")]
    public static (OperationResult, List<LogMessageObject>) GetLogs()
    {
        Core.Core.CheckInit();
        
        var (result, data) = Core.Core.GetService<ILogService>().GetLogs();
        
        return (result, data);
    }

    public string GetModuleHtml()
    {
        return "<h1>Logging Module</h1>";
    }
}