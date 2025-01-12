namespace LogicLayer.CoreModels;

public class OperationResult(int code = 200, string message = "OK")
{
    public int Code { get; set; } = code;
    public string Message { get; set; } = message;


    public static OperationResult GetSuccess()
    {
        return new OperationResult();
    }
    
    public static OperationResult GetCreated(string message = "Created")
    {
        return new OperationResult(201, message);
    }
    
    public static OperationResult GetNotModified(string message = "Not Modified")
    {
        return new OperationResult(304, message);
    }

    public static OperationResult GetBadRequest(string message = "BadRequest")
    {
        return new OperationResult(400, message);
    }
    
    public static OperationResult GetUnauthorized(string message = "Unauthorized")
    {
        return new OperationResult(401, message);
    }

    public static OperationResult GetNotFound(string message = "Not Found")
    {
        return new OperationResult(404, message);
    }
}