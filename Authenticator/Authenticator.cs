using LogicLayer;
using LogicLayer.Authentication;
using LogicLayer.CoreModels;

namespace Authenticator;

public static class Authenticator
{
    public static (List<AuthPermissionClaim>, string) GetAuthenticationPermissions(Dictionary<string, string> headers)
    {
        if (headers.TryGetValue("ApiKey", out var apiKey))
        {
            return AuthenticationCore.GetPermissionsFromApiKey(apiKey);
        }
        if (headers.TryGetValue("Bearer", out var bearer))
        {
            return AuthenticationCore.GetPermissionsFromBearer(bearer);
        }

        return ([], "");
    }
}