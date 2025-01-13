using AzureDatabase.Interfaces;
using LogicLayer.Authentication.Interfaces;
using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.Data.SqlClient;

namespace AzureDatabase.Services;

public class AzureAuthenticationService : AzureDatabaseService, IAuthenticationService
{
    private const int MaxRetries = 3;
    
    public async Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromApiKey(string apiKey, int retryCount = 0)
    {
        try
        {
            var token = "";
            var permissions = new List<AuthPermissionClaim>();
            var client = "";
            var duration = new DateTime();

            await using var command = new SqlCommand($"SELECT Token, Permissions, Client, Duration FROM Permissions WHERE Token = 'apikey-{apiKey}'", new DatabaseConnection().Connection);
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
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return ([], "");
            
            return await GetPermissionsFromApiKey(apiKey, retryCount + 1); // Retry after creating the table
        }
    }

    public async Task<(List<AuthPermissionClaim>, string)> GetPermissionsFromBearer(string bearer, int retryCount = 0)
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
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return ([], "");
            
            return await GetPermissionsFromBearer(bearer, retryCount + 1); // Retry after creating the table
        }
    }

    public async Task<DateTime> CreateBearerToken(string token, string permissionString, string client, int retryCount = 0)
    {
        var expires = DateTime.Now.AddDays(7);
        
        try
        {
            await using var command = new SqlCommand("INSERT INTO Permissions (Token, Permissions, Client, Duration) VALUES (@Token, @Permissions, @Client, @Duration)", new DatabaseConnection().Connection);
            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@Permissions", permissionString);
            command.Parameters.AddWithValue("@Client", client);
            command.Parameters.AddWithValue("@Duration", expires);
            command.ExecuteNonQuery();
            return expires;
        }
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return new DateTime();
            
            return await CreateBearerToken(token, permissionString, client, retryCount + 1); // Retry after creating the table
        }
    }

    public async Task<(string, List<PermissionClaim>, string)> GetUser(string username, int retryCount = 0)
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
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return ("", [], "");
            
            return await GetUser(username, retryCount + 1); // Retry after creating the table
        }
    }

    private async Task RemoveExpiredToken(string token, int retryCount = 0)
    {
        try
        {
            await using var command = new SqlCommand("DELETE FROM Permissions WHERE Token = '" + token + "'", new DatabaseConnection().Connection);
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return;
            
            await RemoveExpiredToken(token, retryCount + 1); // Retry after creating the table
        }
    }

    protected override async Task CreateTable(int retryCount = 0)
    {
        try
        {
            await using var command = new SqlCommand(@"
            CREATE TABLE Permissionss (
                token VARCHAR(MAX),
                Permissions VARCHAR(MAX),
                Client VARCHAR(MAX),
                Duration DATETIME
            )", new DatabaseConnection().Connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating table: {ex.Message}");
            if (retryCount < MaxRetries)
            {
                await CreateTable(retryCount + 1); // Retry creating the table
            }
        }
    }
}