namespace ModuleBuilder.Models.Components;

public class Button( string id, string placeholder, Action action) : IComponent
{
    public string ConvertToHtml()
    {
        return $"<button id='{id}'>{placeholder}</button>" + GetActionScript();
    }
    
    private string GetActionScript()
    {
        return $"<script>document.getElementById('{id}').setAttribute('onclick', `" + action.ConvertToHtml() + "`);</script>";
    }
}