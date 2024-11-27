using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace LogicLayer.Core;

public static partial class Core
{
    public static OperationResult CreateLog(LogMessageObject logObject)
    {
        CheckInit();
        
        GetService<ILogService>().CreateLog(logObject);
        
        return OperationResult.GetSuccess();
    }
    
    public static OperationResult CreateLogTwice(LogMessageObject logObject)
    {
        CheckInit();
        
        GetService<ILogService>().CreateLogTwice(logObject);
        
        return OperationResult.GetSuccess();
    }
}