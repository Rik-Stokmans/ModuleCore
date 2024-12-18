using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace AzureDatabase
{
    public class DatabaseConnection : IAsyncDisposable
    {
        public readonly SqlConnection Connection;

        public DatabaseConnection()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            try
            {
                Connection.Open();
                Console.WriteLine("Connection to Azure SQL Database successful.");
            }
            catch (SqlException exception)
            {
                Console.WriteLine("Error: " + exception.Message);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Connection.CloseAsync();
            Console.WriteLine("Connection closed.");
        }
    }
}