using AzureDatabase.Interfaces;
using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.Data.SqlClient;

namespace AzureDatabase.Services;

public class AzureLogService : AzureDatabaseService, ILogService
{
    private const int MaxRetries = 3;

    public async Task<OperationResult> CreateLog(LogMessageObject logObject, int retryCount = 0)
    {
        try
        {
            await using var command = new SqlCommand("INSERT INTO Logs (Message, Timestamp) VALUES (@Message, @Timestamp)", new DatabaseConnection().Connection);
            command.Parameters.AddWithValue("@Message", logObject.Message);
            command.Parameters.AddWithValue("@Timestamp", logObject.Time);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0
                ? OperationResult.GetSuccess()
                : OperationResult.GetBadRequest();
        }
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return OperationResult.GetBadRequest();
            
            return await CreateLog(logObject, retryCount + 1); // Retry after creating the table
        }
    }

    public async Task<(OperationResult, List<LogMessageObject>)> GetLogs(int retryCount = 0)
    {
        try
        {
            var logs = new List<LogMessageObject>();

            await using var command = new SqlCommand("SELECT Message, Timestamp FROM Logs", new DatabaseConnection().Connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                logs.Add(new LogMessageObject(
                    reader.GetString(0),
                    reader.GetDateTime(1)
                ));
            }

            return (OperationResult.GetSuccess(), logs);
        }
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return (OperationResult.GetBadRequest(), []);
            
            return await GetLogs(retryCount + 1); // Retry after creating the table
        }
    }


    public async Task<OperationResult> DeleteLogs(int retryCount = 0)
    {
        try
        {
            await using var command = new SqlCommand("DELETE FROM Logs", new DatabaseConnection().Connection);
            var rowsAffected = command.ExecuteNonQuery();
            return await Task.FromResult(rowsAffected > 0
                ? OperationResult.GetSuccess()
                : OperationResult.GetBadRequest("No logs to delete"));
        }
        catch (SqlException ex)
        {
            if (!await HandleExceptions(ex, retryCount)) return OperationResult.GetBadRequest();
            
            return await DeleteLogs(retryCount + 1); // Retry after creating the table
        }
    }
    
    
    //Interface implementation
    protected override async Task CreateTable(int retryCount = 0)
    {
        try
        {
            await using var command = new SqlCommand(@"
            CREATE TABLE Logs (
                Timestamp DATETIME,
                Message NVARCHAR(MAX)
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