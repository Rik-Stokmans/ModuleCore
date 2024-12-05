using LogicLayer.Authentication.Interfaces;

namespace MockDataLayer.Services;

public class AuthenticationService : IAuthenticationService
{
    public bool ApiKeyIsAuthenticated(string apiKey)
    {
        return MockData.ApiKeys.Contains(apiKey);
    }

    public List<string> GetAllSerialNumbers()
    {
        return MockData.SerialNumbers;
    }
}