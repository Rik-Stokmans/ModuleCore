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
            var token = "";
            var permissions = new List<AuthPermissionClaim>();
            var client = "";
            var duration = new DateTime();

            await using var command = new SqlCommand($"SELECT Token, Permissions, Client, Duration FROM Permissions WHERE Token = 'ApiKey-{apiKey}'", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                token = reader.GetString(0);
                reader.GetString(1).Split(',').ToList().ForEach(permission =>
                {
                    permissions.Add(new AuthPermissionClaim((PermissionClaim) int.Parse(permission)));
                });
                client = reader.GetString(2);
                duration = reader.GetDateTime(3);
                Console.WriteLine(duration);
            }
            
            if (duration < DateTime.Now)
            {
                await RemoveExpiredToken(token);
                return ([], "");
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
            var token = "";
            var permissions = new List<AuthPermissionClaim>();
            var client = "";
            var duration = new DateTime();

            await using var command = new SqlCommand($"SELECT Token, Permissions, Client, Duration FROM Permissions WHERE Token = 'Bearer-{bearer}'", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                token = reader.GetString(0);
                reader.GetString(1).Split(',').ToList().ForEach(permission =>
                {
                    permissions.Add(new AuthPermissionClaim((PermissionClaim) int.Parse(permission)));
                });
                client = reader.GetString(2);
                duration = reader.GetDateTime(3);
            }
            
            if (duration < DateTime.Now)
            {
                await RemoveExpiredToken(token);
                return ([], "");
            }

            return (permissions, client);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ([], "");
        }
    }

    public bool CreateBearerToken(string token, string permissionString, string client)
    {
        try
        {
            using var command = new SqlCommand("INSERT INTO Permissions (Token, Permissions, Client, Duration) VALUES (@Token, @Permissions, @Client, @Duration)", new DatabaseConnection().Connection);
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@Permissions", permissionString);
            command.Parameters.AddWithValue("@Client", client);
            command.Parameters.AddWithValue("@Duration", DateTime.Now.AddDays(7));
            command.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<(string, List<PermissionClaim>, string)> GetUser(string username)
    {
        try
        {
            var password = "";
            List<PermissionClaim> permissions = [];
            var client = "";
            
            await using var command = new SqlCommand("SELECT Password, Permissions, Client FROM Users WHERE Username = '" + username + "'", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                password = reader.GetString(0);
                reader.GetString(1).Split(',').ToList().ForEach(permission =>
                {
                    permissions.Add((PermissionClaim) int.Parse(permission));
                });
                client = reader.GetString(2);
            }

            return (password, permissions, client);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ("", [], "");
        }
    }

    private async Task RemoveExpiredToken(string token)
    {
        try
        {
            await using var command = new SqlCommand("DELETE FROM Permissions WHERE Token = '" + token + "'", new DatabaseConnection().Connection);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}