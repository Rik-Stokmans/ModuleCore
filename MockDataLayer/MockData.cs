using LogicLayer.Modules.ChildFocusModule.Models;
using LogicLayer.Modules.LoggingModule.Models;

namespace MockDataLayer;

public static class MockData
{
    public static List<LogMessageObject> LogObjects { get; set; } = [];
    
    public static List<string> ApiKeys { get; set; } = ["ApiKeyTest"];
    
    public static List<string> SerialNumbers { get; set; } = ["SerialNumberTest"];
    
    public static List<ChildFocusLog> ChildFocusLogs { get; set; } = [];
    
    public static List<MissingPerson> MissingPersons { get; set; } = [];
}