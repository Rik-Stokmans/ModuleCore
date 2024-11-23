using LogicLayer.Interfaces;
using LogicLayer.Models;

namespace MockDataLayer.Services;

public class LogMockService : ILogService
{
    public OperationResult CreateLog(string message)
    {
        Console.WriteLine($"Log: {message}");
        
        return OperationResult.GetSuccess();
    }
}
