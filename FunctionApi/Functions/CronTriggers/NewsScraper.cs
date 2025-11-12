using System.Text;
using System.Text.Json;
using FunctionApi.Functions.HttpTriggers;
using LogicLayer.Modules.NewsScraperModule.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunctionApi.Functions.CronTriggers;

public class NewsScraper(ILogger<Login> logger, INewsObjectService newsObjectService, IConfiguration configuration)
{
    private readonly HttpClient _httpClient = new();

    [Function("NewsScraper")]
    public async Task Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
    {
        logger.LogInformation("C# Timer trigger function executed at: {0}", DateTime.Now);
        
        try
        {
            // Fetch the news objects
            var newsObjects = await newsObjectService.GetNewsObjects();

            // Format the data as per the required structure
            var payload = new
            {
                data = new { dataArray = newsObjects },
                event_id = "289478969"
            };

            // Serialize the payload to JSON
            var jsonPayload = JsonSerializer.Serialize(payload);

            // Prepare the API request
            var endpointUrl = "https://editor.xasting.com/public-api/integration/webhooks";
            var apiKey = configuration.GetSection("XastWebhookApiKey");
            
            Console.WriteLine(apiKey);

            var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
            {
                Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            };

            // Add the API key to the headers
            request.Headers.Add("x-webhook-apikey", $"{apiKey}");

            // Send the request
            var response = await _httpClient.SendAsync(request);

             if (response.IsSuccessStatusCode)
             {
                 logger.LogInformation("Data successfully sent to the API.");
             }
             else
             {
                 logger.LogError($"Failed to send data to the API. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
             }
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occurred while sending data: {ex.Message}");
        }
    }
}
