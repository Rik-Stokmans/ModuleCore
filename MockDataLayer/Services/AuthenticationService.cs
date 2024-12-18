using LogicLayer.Authentication.Interfaces;

namespace MockDataLayer.Services;

public class AuthenticationService : IAuthenticationService
{
    public Task<bool> ApiKeyIsAuthenticated(string apiKey)
    {
        return Task.FromResult(MockData.ApiKeys.Contains(apiKey));
    }

    public Task<List<string>> GetAllSerialNumbers()
    {
        return Task.FromResult(MockData.SerialNumbers);
    }
}