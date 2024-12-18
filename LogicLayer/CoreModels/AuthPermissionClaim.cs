namespace LogicLayer.CoreModels;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AuthPermissionClaim(PermissionClaim permissionClaim) : Attribute
{
    PermissionClaim PermissionClaim { get; } = permissionClaim;
}

public enum PermissionClaim
{
    Post = 0,
    Get = 1,
    Delete = 2,
    Frontend = 3
}