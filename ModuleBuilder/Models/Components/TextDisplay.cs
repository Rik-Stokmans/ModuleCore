namespace ModuleBuilder.Models.Components;

public class TextDisplay(string id, string text) : IComponent
{
    private string Text { get; set; } = text;
    
    public string ConvertToHtml()
    {
        return $"<p id='{id}'>{Text}</p>";
    }
}