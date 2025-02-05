namespace ModuleBuilder.Models;

public class Widget(List<IComponent> components)
{
    
    private List<IComponent> Components { get; set; } = components;

    public string ConvertToHtml()
    {
        var html = "<div class='module-widget'>";
        foreach (var component in Components)
        {
            html += component.ConvertToHtml();
        }
        html += "</div>";
        return html;
    }
}