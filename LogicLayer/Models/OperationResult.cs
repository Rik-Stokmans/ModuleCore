namespace LogicLayer.Models;

public class OperationResult(int code = 200, string message = "OK")
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;


    public static OperationResult GetSuccess()
    {
        return new OperationResult();
    }
    
    public static OperationResult GetCreated()
    {
        return new OperationResult(201, "Created");
    }
    
    public static OperationResult GetNotModified()
    {
        return new OperationResult(304, "Not Modified");
    }

    public static OperationResult GetBadRequest()
    {
        return new OperationResult(400, "BadRequest");
    }

    public static OperationResult GetNotFound()
    {
        return new OperationResult(404, "Not Found");
    }
}