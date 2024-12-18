using LogicLayer.CoreModels;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Models;

namespace MockDataLayer.Services;

public class ChildFocusMockService : IChildFocus
{
    public OperationResult LogChildFocus(ChildFocusLog logLog)
    {
        MockData.ChildFocusLogs.Add(logLog);
        
        return OperationResult.GetSuccess();
    }

    public OperationResult LogMissingPerson(MissingPerson missingPerson)
    {
        MockData.MissingPersons.Add(missingPerson);
        
        return OperationResult.GetSuccess();
    }

    public (OperationResult, List<ChildFocusLog>) GetChildFocusLogs()
    {
        return (OperationResult.GetSuccess(), MockData.ChildFocusLogs);
    }

    public (OperationResult, List<MissingPerson>) GetMissingPersons()
    {
        return (OperationResult.GetSuccess(), MockData.MissingPersons);
    }

    public bool MissingPersonExists(int missingPersonsId)
    {
        return MockData.MissingPersons.Any(x => x.Id == missingPersonsId);
    }
}