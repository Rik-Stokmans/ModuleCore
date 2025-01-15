using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Models;
using LogicLayer.Modules.ModuleCollectionBuilder.Models;

namespace LogicLayer.Modules.LoggingModule;

public class LoggingModule : IModule
{
    [AuthPermissionClaim(PermissionClaim.Post)]
    [HttpMethod("POST")]
    public static OperationResult CreateLog(LogMessageObject logObject)
    {
        var result = Core.GetService<ILogService>().CreateLog(logObject).Result;
        
        return result;
    }

    [AuthPermissionClaim(PermissionClaim.Get)]
    [HttpMethod("GET")]
    public static (OperationResult, List<LogMessageObject>) GetLogs()
    {
        var (result, data) = Core.GetService<ILogService>().GetLogs().Result;
        
        return (result, data);
    }

    [AuthPermissionClaim(PermissionClaim.Delete)]
    [HttpMethod("DELETE")]
    public static OperationResult DeleteLogs()
    {
        var result = Core.GetService<ILogService>().DeleteLogs().Result;
        
        return result;
    }

    public ModuleHtmlObject GetModuleHtml()
    {
        return new ModuleHtmlObject
        ( 
            "Logging",
            """
            <h1>Logging Module</h1>
            <p>Use the buttons below to interact with the Logging module:</p>
            
            <!-- Input field for custom log message -->
            <div>
                <label for="log-message">Log Message:</label>
                <input type="text" id="log-message" placeholder="Enter your log message here">
            </div>
            
            <button id='create-log'>Create Log</button>
            <button id='get-logs'>Get Logs</button>
            <button id='delete-logs'>Delete Logs</button>
            
            <div id='response-area'>
                <h2>Response</h2>
                <pre id='response-output'></pre>
            </div>
            
            <script>
                function getCookie(name) {
                    const value = `; ${document.cookie}`;
                    const parts = value.split(`; ${name}=`);
                    if (parts.length === 2) return parts.pop().split(';').shift();
                    return null;
                }
            
                function getBearerToken() {
                    console.log("Getting Bearer token from cookies");
                    // Get the Bearer token from cookies
                    const bearerToken = getCookie("BearerToken");
                
                    console.log("Bearer token:", bearerToken);
                
                    if (!bearerToken) {
                        window.location.href = "http://localhost:63343/ModuleCoreFrontend/login.html";
                    }
                
                    return bearerToken;
                }
                
                (function () {
                    console.log("Logging Module Script Loaded");
            
                    // Define API endpoint
                    const apiEndpoint = 'http://localhost:7191/api';
            
                    // Helper function to get log message from input field
                    function getLogMessage() {
                        const inputField = document.getElementById('log-message');
                        return inputField.value.trim() || "Default log message"; // Fallback if input is empty
                    }
            
                    // Attach event handlers
                    document.getElementById('create-log').addEventListener('click', async () => {
                        const logObject = {
                            "LogObject": {
                                "Message": getLogMessage()
                            }
                        };
                        try {
                            const response = await fetch(`${apiEndpoint}/CreateLog`, {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json',
                                    'Bearer': getBearerToken()
                                },
                                body: JSON.stringify(logObject)
                            });
                            if (!response.ok) {
                                throw new Error(`Failed to create log: ${response.statusText}`);
                            }
                            const result = await response.json();
                            document.getElementById('response-output').textContent = JSON.stringify(result, null, 2);
                        } catch (err) {
                            console.error('Error creating log:', err);
                            document.getElementById('response-output').textContent = 'Error creating log: ' + err.message;
                        }
                    });
            
                    document.getElementById('get-logs').addEventListener('click', async () => {
                        try {
                            const response = await fetch(`${apiEndpoint}/GetLogs`, {
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
                            document.getElementById('response-output').textContent = JSON.stringify(result, null, 2);
                        } catch (err) {
                            console.error('Error fetching logs:', err);
                            document.getElementById('response-output').textContent = 'Error fetching logs: ' + err.message;
                        }
                    });
            
                    document.getElementById('delete-logs').addEventListener('click', async () => {
                        try {
                            const response = await fetch(`${apiEndpoint}/DeleteLogs`, {
                                method: 'DELETE',
                                headers: {
                                    'Content-Type': 'application/json',
                                    'Bearer': getBearerToken()
                                }
                            });
                            if (!response.ok) {
                                throw new Error(`Failed to delete logs: ${response.statusText}`);
                            }
                            const result = await response.json();
                            document.getElementById('response-output').textContent = JSON.stringify(result, null, 2);
                        } catch (err) {
                            console.error('Error deleting logs:', err);
                            document.getElementById('response-output').textContent = 'Error deleting logs: ' + err.message;
                        }
                    });
                })();
            </script>
            """
        );
    }
}