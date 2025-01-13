using LogicLayer.CoreModels;

namespace LogicLayer.Authentication.Interfaces;

public interface IAuthenticationService
{
    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromApiKey(string apiKey, int retryCount = 0);

    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromBearer(string bearer, int retryCount = 0);
    public Task<(string password, List<PermissionClaim> permissionClaims, string client)> GetUser(string username, int retryCount = 0);
    public Task<DateTime> CreateBearerToken(string token, string permissions, string client, int retryCount = 0);
}
    
    