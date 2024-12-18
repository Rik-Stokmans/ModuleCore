namespace LogicLayer.Authentication.Interfaces;

public interface IAuthenticationService
{
    public Task<bool> ApiKeyIsAuthenticated(string apiKey);
    
    public Task<List<string>>GetAllSerialNumbers();
}