namespace ModuleBuilder.Models;

public class Module(List<Widget> widgets, string title)
{
    private List<Widget> Widgets { get; set; } = widgets;
    private string Title { get; set; } = title;
    
    
    public ModuleHtmlObject ConvertToModuleHtmlObject()
    {
        var sideBarHtml = title;
        
        var viewerHtml = "<div class='module-container'>";
        foreach (var widget in Widgets)
        {
            viewerHtml += widget.ConvertToHtml();
        }
        viewerHtml += "</div>";
        
        return new ModuleHtmlObject(sideBarHtml, viewerHtml);
    }
}