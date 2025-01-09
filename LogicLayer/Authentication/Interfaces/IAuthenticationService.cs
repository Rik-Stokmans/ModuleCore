using LogicLayer.CoreModels;

namespace LogicLayer.Authentication.Interfaces;

public interface IAuthenticationService
{
    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromApiKey(string apiKey);

    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromBearer(string bearer);
    public Task<(string password, List<PermissionClaim> permissionClaims, string client)> GetUser(string username);
    public bool CreateBearerToken(string token, string permissions, string client);
}
    
    