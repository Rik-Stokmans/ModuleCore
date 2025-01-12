using Microsoft.Data.SqlClient;

namespace AzureDatabase.Interfaces;

public abstract class AzureDatabaseService
{
    private const int MaxRetries = 3;
    protected abstract Task CreateTable(int retryCount = 0);
    protected async Task<bool> HandleExceptions(SqlException ex, int retryCount = 0)
    {
        if (ex.Number != 208 || retryCount >= MaxRetries) return false;
            
        await CreateTable();
        return true;
    }
}