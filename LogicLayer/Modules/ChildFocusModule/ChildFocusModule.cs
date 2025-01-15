using LogicLayer.CoreModels;
using LogicLayer.Modules.ChildFocusModule.Interfaces;
using LogicLayer.Modules.ChildFocusModule.Methods;
using LogicLayer.Modules.ChildFocusModule.Models;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.ModuleCollectionBuilder.Models;

namespace LogicLayer.Modules.ChildFocusModule;

public class ChildFocusModule : IModule
{
    [AuthPermissionClaim(PermissionClaim.Post)]
    [HttpMethod("POST")]
    public static OperationResult LogChildFocus(ChildFocusLog childFocusLog)
    {
        OperationResult result;

        var missingPersonExists = Core.GetService<IChildFocus>().MissingPersonExists(childFocusLog.ChildFocusId);
        if (missingPersonExists)
        {
            result = Core.GetService<IChildFocus>().LogChildFocus(childFocusLog);
            return result;
        }
        
        // Fetch the child focus object from the public API
        var missingPersonObject = Methods.Methods.FetchChildFocusObject(childFocusLog.ChildFocusId);
        if (missingPersonObject == null) return OperationResult.GetNotFound();
        
        // Log the fetched object
        Core.GetService<IChildFocus>().LogMissingPerson(missingPersonObject);
        result = Core.GetService<IChildFocus>().LogChildFocus(childFocusLog);

        return result;
    }
    
    [AuthPermissionClaim(PermissionClaim.Get)]
    [HttpMethod("GET")]
    public static (OperationResult, List<ChildFocusLog>) GetChildFocusLogs()
    {
        var result = Core.GetService<IChildFocus>().GetChildFocusLogs();
        
        return result;
    }
    
    [AuthPermissionClaim(PermissionClaim.Get)]
    [HttpMethod("GET")]
    public static (OperationResult, List<MissingPerson>) GetChildFocusObjects()
    {
        var result = Core.GetService<IChildFocus>().GetMissingPersons();
        
        return result;
    }

    public ModuleHtmlObject GetModuleHtml()
    {
        return new ModuleHtmlObject
        ( 
            "ChildFocus",
            """
            <h1>ChildFocus Module</h1>
            <p>Use the buttons below to interact with the ChildFocus module:</p>
            
            <!-- Buttons to interact with the API -->
            <div>
                <button id="fetch-logs">Fetch ChildFocus Logs</button>
                <button id="fetch-objects">Fetch ChildFocus Objects</button>
            </div>
            
            <!-- Output area for displaying responses -->
            <div id="response-area">
                <h2>Response</h2>
                <pre id="response-output"></pre>
            </div>
            
            <script>
                (function () {
                    console.log("ChildFocus Module Script Loaded");
            
                    // Define API endpoint
                    const apiEndpoint = 'http://localhost:7191/api';
            
                    // Helper function to update the response area
                    function updateResponseArea(content) {
                        document.getElementById('response-output').textContent = content;
                    }
            
                    // Attach event handlers
                    document.getElementById('fetch-logs').addEventListener('click', async () => {
                        try {
                            const response = await fetch(`${apiEndpoint}/GetChildFocusLogs`, {
                                method: 'GET',
                                headers: {
                                    'Content-Type': 'application/json',
                                    'Bearer': getBearerToken()
                                }
                            });
                            if (!response.ok) {
                                throw new Error(`Failed to fetch logs: ${response.statusText}`);
                            }
                            const result = await response.json();
                            updateResponseArea(JSON.stringify(result, null, 2)); // Pretty-print JSON
                        } catch (err) {
                            console.error('Error fetching ChildFocus logs:', err);
                            updateResponseArea('Error fetching logs: ' + err.message);
                        }
                    });
            
                    document.getElementById('fetch-objects').addEventListener('click', async () => {
                        try {
                            const response = await fetch(`${apiEndpoint}/GetChildFocusObjects`, {
                                method: 'GET',
                                headers: {
                                    'Content-Type': 'application/json',
                                    'Bearer': getBearerToken()
                                }
                            });
                            if (!response.ok) {
                                throw new Error(`Failed to fetch objects: ${response.statusText}`);
                            }
                            const result = await response.json();
                            updateResponseArea(JSON.stringify(result, null, 2)); // Pretty-print JSON
                        } catch (err) {
                            console.error('Error fetching ChildFocus objects:', err);
                            updateResponseArea('Error fetching objects: ' + err.message);
                        }
                    });
                })();
            </script>
            """
        );
    }
}