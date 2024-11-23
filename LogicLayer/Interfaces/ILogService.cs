using LogicLayer.Models;

namespace LogicLayer.Interfaces;

public interface ILogService
{
    [HttpMethod("GET")]
    public OperationResult CreateLog(string message);
    
    /*
    [HttpMethod("GET")]
    public string GetStatus(dynamic? inputData);
    */
}