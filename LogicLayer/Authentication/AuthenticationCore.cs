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

    public static async Task<(bool succes, string token, DateTime expires)> LoginUser(string username, string password)
    {
        string dbPassword;
        List<PermissionClaim> permissions;
        string client;
        
        try
        {
            (dbPassword, permissions, client) = Core.GetService<IAuthenticationService>().GetUser(username).Result;
        }
        catch (Exception e)
        {
            return (false, e.ToString(), new DateTime());
        }

        if (dbPassword == password)
        {
            var (token, expires) = await CreateBearerToken(permissions, client);
            
            return (true, token, expires);
        }
        
        return (false, "", new DateTime());
    }

    private static async Task<(string, DateTime)> CreateBearerToken(List<PermissionClaim> permissions, string client)
    {
        var token = Guid.NewGuid().ToString();
        var permissionString = string.Join(',', permissions.Select(permission => (int) permission));
        
        var expires = await Core.GetService<IAuthenticationService>().CreateBearerToken("bearer-" + token, permissionString, client);
        
        return (token, expires);
    }
}