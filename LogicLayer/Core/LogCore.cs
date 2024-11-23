using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace LogicLayer.Core;

public static partial class Core
{
    public static OperationResult CreateLog(string message)
    {
        CheckInit();
        
        GetService<ILogService>().CreateLog(message);
        
        return OperationResult.GetSuccess();
    }
}