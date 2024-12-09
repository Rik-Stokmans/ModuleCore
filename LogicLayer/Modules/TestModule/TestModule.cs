using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Models;
using LogicLayer.Modules.ModuleCollectionBuilder.Models;
using LogicLayer.Modules.TestModule.Interfaces;
using LogicLayer.Modules.TestModule.Models;

namespace LogicLayer.Modules.TestModule;

public class TestModule : IModule
{
    [HttpMethod("POST")]
    public static OperationResult CreateTestObject(TestObject testObject)
    {
        Core.CheckInit();
        
        var result = Core.GetService<ITestService>().CreateTestObject(testObject);
        
        return result;
    }

    [HttpMethod("GET")]
    public static (OperationResult, List<TestObject>) GetTestObjects()
    {
        Core.CheckInit();
        
        var (result, data) = Core.GetService<ITestService>().GetTestObjects();
        
        return (result, data);
    }

    public ModuleHtmlObject GetModuleHtml()
    {
        return new ModuleHtmlObject
        ( 
            "Test",
            "<h1>This is a test</h1>"
        );
    }
}