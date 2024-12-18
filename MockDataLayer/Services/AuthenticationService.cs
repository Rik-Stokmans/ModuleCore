using LogicLayer.Authentication.Interfaces;
using LogicLayer.CoreModels;

namespace MockDataLayer.Services;

public class AuthenticationService : IAuthenticationService
{
    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromApiKey(string apiKey)
    {
        throw new NotImplementedException();
    }

    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromBearer(string bearer)
    {
        throw new NotImplementedException();
    }
}