namespace LogicLayer.Modules.ModuleCollectionBuilder.Models;

public class ModuleHtmlObject(string sidebarHtml, string viewerHtml)
{
    public string SidebarHtml { get; set; } = sidebarHtml;
    
    public string ViewerHtml { get; set; } = viewerHtml;
}