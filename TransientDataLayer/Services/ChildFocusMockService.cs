using LogicLayer.CoreModels;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Models;

namespace MockDataLayer.Services;

public class ChildFocusMockService : IChildFocus
{
    public static List<ChildFocusLog> ChildFocusLogs { get; set; } = [];
    
    public static List<MissingPerson> MissingPersons { get; set; } = [];
    
    public OperationResult LogChildFocus(ChildFocusLog logLog)
    {
        ChildFocusLogs.Add(logLog);
        
        return OperationResult.GetSuccess();
    }

    public OperationResult LogMissingPerson(MissingPerson missingPerson)
    {
        MissingPersons.Add(missingPerson);
        
        return OperationResult.GetSuccess();
    }

    public (OperationResult, List<ChildFocusLog>) GetChildFocusLogs()
    {
        return (OperationResult.GetSuccess(), ChildFocusLogs);
    }

    public (OperationResult, List<MissingPerson>) GetMissingPersons()
    {
        return (OperationResult.GetSuccess(), MissingPersons);
    }

    public bool MissingPersonExists(int missingPersonsId)
    {
        return MissingPersons.Any(x => x.Id == missingPersonsId);
    }
}