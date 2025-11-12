namespace ModuleBuilder.Models.Components;

public class TextInput(string id) : IComponent
{
    public string ConvertToHtml()
    {
        return $"<input id='{id}' type='text'>";
    }
}