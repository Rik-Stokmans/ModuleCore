using System.Data;
using LogicLayer.CoreModels;
using LogicLayer.Modules.LoggingModule.Interfaces;
using LogicLayer.Modules.LoggingModule.Models;
using Microsoft.Data.SqlClient;

namespace AzureDatabase.Services;

public class AzureLogService : ILogService
{
    public async Task<OperationResult> CreateLog(LogMessageObject logObject)
    {
        try
        {
            await using var command = new SqlCommand("INSERT INTO Logs (Message, Timestamp) VALUES (@Message, @Timestamp)", new DatabaseConnection().Connection);
            command.Parameters.AddWithValue("@Message", logObject.Message);
            command.Parameters.AddWithValue("@Timestamp", logObject.Time);

            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0
                ? OperationResult.GetSuccess()
                : OperationResult.GetBadRequest();
        }
        catch (Exception ex)
        {
            return OperationResult.GetBadRequest();
        }
    }

    public async Task<(OperationResult, List<LogMessageObject>)> GetLogs()
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return (OperationResult.GetBadRequest(), []);
        }
    }


    public Task<OperationResult> DeleteLogs()
    {
        try
        {
            using var command = new SqlCommand("DELETE FROM Logs", new DatabaseConnection().Connection);
            var rowsAffected = command.ExecuteNonQuery();
            return Task.FromResult(rowsAffected > 0
                ? OperationResult.GetSuccess()
                : OperationResult.GetBadRequest());
        }
        catch (Exception ex)
        {
            return Task.FromResult(OperationResult.GetBadRequest());
        }
    }
}