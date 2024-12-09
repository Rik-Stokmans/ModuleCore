using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;
using LogicLayer.Modules.TestModule.Models;

namespace LogicLayer.Modules.TestModule.Interfaces;

public interface ITestService
{
    public OperationResult CreateTestObject(TestObject testObject);
    
    public (OperationResult, List<TestObject>) GetTestObjects();
}