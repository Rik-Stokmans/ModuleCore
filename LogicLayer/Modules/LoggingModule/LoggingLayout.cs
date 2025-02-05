using ModuleBuilder.Models;
using ModuleBuilder.Models.Components;
using Action = ModuleBuilder.Models.Components.Action;

namespace LogicLayer.Modules.LoggingModule;

public class LoggingLayout
{
    public static ModuleHtmlObject GetModuleHtml()
    {
        var consoleLogAction = new Action("""console.log("Button clicked");""");
        var noAction = new Action("");
        var updateChart = new Action("""
                                         myChart.data.datasets[0].data = [Math.floor(Math.random() * 10) + 1,Math.floor(Math.random() * 10) + 1,Math.floor(Math.random() * 10) + 1,Math.floor(Math.random() * 10) + 1,Math.floor(Math.random() * 10) + 1]; 
                                         myChart.update();
                                     """);
        var fetchLogsAction = new Action("""
                                             fetch('http://localhost:5184/api/Logs/20', {
                                                 method: 'GET',
                                                 headers: {'Content-Type': 'application/json'},
                                                 credentials: 'include',
                                             })
                                             .then(response => response.json())
                                             .then(data => {
                                                 document.getElementById('textDisplay').innerText = JSON.stringify(data);
                                             });
                                         """);
        var postLogAction = new Action("""
                                       
                                       let message = document.getElementById('text-input').value;
                                       
                                       fetch('http://localhost:5184/api/Logs/' + message, {
                                           method: 'POST',
                                           headers: {'Content-Type': 'application/json'},
                                           credentials: 'include'
                                       })
                                       .then(response => response.json())
                                       .then(data => {
                                           document.getElementById('textDisplay').innerText = JSON.stringify(data);
                                       });
                                       """);

        var container = new Module(
        [
            new Widget(
            [
                new Button("chart-button", "Click me1", updateChart),
                new Chart("myChart", new Dictionary<string, string>
                {
                    {"Red", "1"},
                    {"Blue", "2"},
                    {"Green", "3"},
                    {"Yellow", "4"},
                    {"Orange", "5"}
                }),
                
                new TextDisplay("textDisplay", "Logs will be displayed here"),
                new Button("logs-button", "Fetch logs", fetchLogsAction),
                new TextInput("text-input"),
                new Button("post-log", "Post log", postLogAction),
            ])
        ], "Logging Module");

        return container.ConvertToModuleHtmlObject();
    }
}