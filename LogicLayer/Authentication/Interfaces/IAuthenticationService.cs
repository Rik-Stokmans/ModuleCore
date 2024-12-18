using LogicLayer.CoreModels;

namespace LogicLayer.Authentication.Interfaces;

public interface IAuthenticationService
{
    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromApiKey(string apiKey);
    
    public Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromBearer(string bearer);
}