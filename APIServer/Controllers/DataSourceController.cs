using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataSourceController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<object>> GetData(string link, string filter = "")
    {
        var data = GetDataFromLink(link);
        
        if (!string.IsNullOrEmpty(filter))
        {
            var filteredData = FilterData(data, filter);
            return filteredData;
        }
        
        return data;
    }
    
    private async Task<object?> GetDataFromLink(string link)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(link);
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(content);
    }

    private object? FilterData(object? data, string filter)
    {
        if (data is JArray array)
        {

        }

        return null;
    }
}

public class Filter(string field, string value)
{
    public string Field { get; set; } = field;
    public string Value { get; set; } = value;
}