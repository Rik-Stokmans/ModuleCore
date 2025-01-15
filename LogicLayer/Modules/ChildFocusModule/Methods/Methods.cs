using System.Text.Json;
using LogicLayer.Modules.ChildFocusModule.Models;

namespace LogicLayer.Modules.ChildFocusModule.Methods;

public static class Methods
{
    public static MissingPerson? FetchChildFocusObject(int id)
    {
        const string apiUrl = "https://fccu.stopchildporno.be/reportings/GetReportings"; // Replace with the actual API URL

        // Create an HttpClientHandler to ignore SSL certificate errors
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
        };

        using var httpClient = new HttpClient(handler);
        try
        {
            // Make a GET request to the API
            var response = httpClient.GetStringAsync(apiUrl).Result;

            // Deserialize the JSON response into a list of objects
            var missingPeople = JsonSerializer.Deserialize<List<MissingPerson>>(response);

            if (missingPeople == null || missingPeople.Count == 0)
                return null;

            // Find the person with the matching ID
            var person = missingPeople.FirstOrDefault(p => p.Id == id);

            if (person != null)
            {
                // Serialize the object back into a string for return
                return person;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
        }

        return null;
    }
}