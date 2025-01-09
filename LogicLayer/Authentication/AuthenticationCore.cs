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

    public static (bool succes, string token) LoginUser(string username, string password)
    {
        var (dbPassword, permissions, client) = Core.GetService<IAuthenticationService>().GetUser(username).Result;
        
        if (dbPassword == password)
        {
            return (true, CreateBearerToken(permissions, client));
        }
        
        return (false, "");
    }

    private static string CreateBearerToken(List<PermissionClaim> permissions, string client)
    {
        var token = Guid.NewGuid().ToString();
        var permissionString = string.Join(',', permissions.Select(permission => (int) permission));
        
        Core.GetService<IAuthenticationService>().CreateBearerToken("bearer-" + token, permissionString, client);
        
        return token;
    }
}