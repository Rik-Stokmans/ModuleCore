namespace ModuleBuilder.Models.Components;

public class Action(string script) : IComponent
{
    private string Script { get; set; } = script;
    
    public string ConvertToHtml()
    {
        return script;
    }
    
    public string convertToScript()
    {
        return $"<script>{script}</script>";
    }
}