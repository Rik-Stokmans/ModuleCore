using LogicLayer.Authentication.Interfaces;
using LogicLayer.CoreModels;

namespace LogicLayer.Authentication;

public static class AuthenticationCore
{
    public static (List<AuthPermissionClaim>, string) GetPermissionsFromApiKey(string apiKey)
    {
        var (permissions, client) = Core.GetService<IAuthenticationService>().GetPermissionsFromApiKey(apiKey).Result;

        return (permissions, client);
    }

    public static (List<AuthPermissionClaim>, string) GetPermissionsFromBearer(string bearer)
    {
        var (permissions, client) = Core.GetService<IAuthenticationService>().GetPermissionsFromBearer(bearer).Result;
        
        return (permissions, client);
    }
}