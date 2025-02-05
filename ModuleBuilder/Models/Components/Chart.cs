namespace ModuleBuilder.Models.Components;

public class Chart(string id, Dictionary<string, string> data) : IComponent
{
    private string Id { get; set; } = id;

    public string ConvertToHtml()
    {
        return "<canvas id='" + Id + "'></canvas>" + GetChartScript();
    }

    private string GetChartScript()
    {
        // Convert C# List to JavaScript array format
        string labelsArray = "[" + string.Join(", ", data.Keys.Select(label => "'" + label + "'")) + "]";
        string dataArray = "[" + string.Join(", ", data.Values) + "]";

        return "<script>" +
               "var ctx = document.getElementById('" + Id + "').getContext('2d');" +
               "var myChart = new Chart(ctx, {" +
               "  type: 'bar'," + // You can change this dynamically if needed
               "  data: {" +
               "    labels: " + labelsArray + "," +
               "    datasets: [{" +
               "      label: 'Sales'," +
               "      data: " + dataArray + "," +
               "      backgroundColor: 'rgba(75, 192, 192, 0.2)'," +
               "      borderColor: 'rgba(75, 192, 192, 1)'," +
               "      borderWidth: 1" +
               "    }]" +
               "  }," +
               "  options: {" +
               "    scales: {" +
               "      y: { beginAtZero: true }" +
               "    }" +
               "  }" +
               "});" +
               "</script>";
    }
}