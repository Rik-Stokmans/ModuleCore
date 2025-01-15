using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AzureDatabase
{
    public class DatabaseConnection : IAsyncDisposable
    {
        public readonly SqlConnection Connection;

        public DatabaseConnection()
        {
            // Building the configuration and adding environment variables
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Optional: Set base path for configuration
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load from appsettings.json (if available)
                .AddEnvironmentVariables() // Correct method to load environment variables
                .Build();

            // Retrieve the connection string from environment variables (if set)
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string not found.");
                return;
            }

            // Use the connection string to establish a SQL connection
            Connection = new SqlConnection(connectionString);

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