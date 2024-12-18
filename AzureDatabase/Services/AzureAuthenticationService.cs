using LogicLayer.Authentication.Interfaces;
using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.Data.SqlClient;

namespace AzureDatabase.Services;

public class AzureAuthenticationService : IAuthenticationService
{
    public async Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromApiKey(string apiKey)
    {
        try
        {
            var permissions = new List<AuthPermissionClaim>();
            var client = "";

            await using var command = new SqlCommand($"SELECT Token, Permissions, Client FROM Permissions WHERE Token = 'ApiKey-{apiKey}'", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
                reader.GetString(1).Split(',').ToList().ForEach(permission =>
                {
                    permissions.Add(new AuthPermissionClaim((PermissionClaim) int.Parse(permission)));
                });
                client = reader.GetString(2);
            }

            return (permissions, client);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ([], "");
        }
    }

    public async Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromBearer(string bearer)
    {
        try
        {
            var permissions = new List<AuthPermissionClaim>();
            var client = "";

            await using var command = new SqlCommand($"SELECT Token, Permissions, Client FROM Permissions WHERE Token = 'Bearer-{bearer}'", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                reader.GetString(1).Split(',').ToList().ForEach(permission =>
                {
                    permissions.Add(new AuthPermissionClaim((PermissionClaim) int.Parse(permission)));
                });
                client = reader.GetString(2);
            }

            return (permissions, client);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ([], "");
        }
    }
}