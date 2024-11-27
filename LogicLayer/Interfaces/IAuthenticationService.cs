namespace LogicLayer.Interfaces;

public interface IAuthenticationService
{
    public bool ApiKeyIsAuthenticated(string apiKey);
    
    public List<string> GetAllSerialNumbers();
}