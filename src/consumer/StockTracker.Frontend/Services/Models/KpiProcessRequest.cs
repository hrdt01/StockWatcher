using System.Text.Json.Serialization;

namespace StockTracker.Frontend.Services.Models;

public class KpiProcessRequest
{
    [JsonPropertyName(name: "Symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName(name: "ProcessDate")]
    public string ProcessDate { get; set; }

    [JsonConstructor]
    public KpiProcessRequest()
    {
        
    }
}