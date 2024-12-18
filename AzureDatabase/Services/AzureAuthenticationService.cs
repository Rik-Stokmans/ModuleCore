using LogicLayer.Authentication.Interfaces;
using LogicLayer.CoreModels;
using Microsoft.Data.SqlClient;

namespace AzureDatabase.Services;

public class AzureAuthenticationService : IAuthenticationService
{
    public async Task<bool> ApiKeyIsAuthenticated(string apiKey)
    {
        try
        {
            await using var command = new SqlCommand("SELECT COUNT(*) FROM ApiKeys WHERE ApiKey = @ApiKey",
                new DatabaseConnection().Connection);
            command.Parameters.AddWithValue("@ApiKey", apiKey);

            var result = (int) (await command.ExecuteScalarAsync() ?? 0);
            return result > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<List<string>> GetAllSerialNumbers()
    {
        try
        {
            var serialNumbers = new List<string>();
            
            await using var command = new SqlCommand("SELECT SerialNumber FROM SerialNumbers", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();
            
            while (reader.Read())
            {
                serialNumbers.Add(reader.GetString(0));
            }
            
            return serialNumbers;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return [];
        }
    }
}