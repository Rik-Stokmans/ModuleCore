using LogicLayer.Modules.LoggingModule.Models;
using LogicLayer.Modules.TestModule.Models;

namespace MockDataLayer;

public static class MockData
{
    public static List<LogMessageObject> LogObjects { get; set; } = [];
    
    public static List<TestObject> TestObjects { get; set; } = [];
    
    public static List<string> ApiKeys { get; set; } = ["ApiKeyTest"];
    
    public static List<string> SerialNumbers { get; set; } = ["SerialNumberTest"];
}