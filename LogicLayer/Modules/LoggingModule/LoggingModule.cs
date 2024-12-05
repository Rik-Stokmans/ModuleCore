using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Models;

namespace LogicLayer.Modules.LoggingModule;

public class LoggingModule : IModule
{
    [HttpMethod("POST")]
    public static OperationResult CreateLog(LogMessageObject logObject)
    {
        Core.CheckInit();
        
        var result = Core.GetService<ILogService>().CreateLog(logObject);
        
        return result;
    }
    
    [HttpMethod("POST")]
    public static OperationResult CreateLogTwice(LogMessageObject logObject)
    {
        Core.CheckInit();
        
        var result = Core.GetService<ILogService>().CreateLogTwice(logObject);

        return result;
    }

    [HttpMethod("GET")]
    public static (OperationResult, List<LogMessageObject>) GetLogs()
    {
        Core.CheckInit();
        
        var (result, data) = Core.GetService<ILogService>().GetLogs();
        
        return (result, data);
    }

    public string GetModuleHtml()
    {
        return "<h1>Logging Module</h1>";
    }
}