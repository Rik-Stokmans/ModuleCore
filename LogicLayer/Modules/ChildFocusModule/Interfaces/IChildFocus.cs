using LogicLayer.CoreModels;
using LogicLayer.Modules.ChildFocusModule.Models;

namespace LogicLayer.Modules.ChildFocusModule.Interfaces;

public interface IChildFocus
{
    public OperationResult LogChildFocus(ChildFocusLog logLog);
    
    public OperationResult LogMissingPerson(MissingPerson missingPerson);
    
    public (OperationResult, List<ChildFocusLog>) GetChildFocusLogs();
    
    public (OperationResult, List<MissingPerson>) GetMissingPersons();
    
    public bool MissingPersonExists(int missingPersonId);
}