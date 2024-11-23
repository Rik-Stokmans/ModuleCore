namespace LogicLayer.Models;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HttpMethodAttribute(string verb) : Attribute
{
    public string Verb { get; } = verb;
}