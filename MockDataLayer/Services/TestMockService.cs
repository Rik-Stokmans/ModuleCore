using LogicLayer.CoreModels;
using LogicLayer.Modules.TestModule.Interfaces;
using LogicLayer.Modules.TestModule.Models;

namespace MockDataLayer.Services;

public class TestMockService : ITestService
{
    public OperationResult CreateTestObject(TestObject testObject)
    {
        MockData.TestObjects.Add(testObject);
        return OperationResult.GetSuccess();
    }

    public (OperationResult, List<TestObject>) GetTestObjects()
    {
        return (OperationResult.GetSuccess(), MockData.TestObjects);
    }
}