using System.Text.Json.Serialization;

namespace StockTracker.Models.FrontendModels;

public class TrackedCompany
{
    [JsonPropertyName(name: "Name")]
    public string Name { get; set; }

    [JsonPropertyName(name: "Symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName(name: "Url")]
    public string Url { get; set; }

    [JsonPropertyName(name: "Enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName(name: "PseudoRowKey")]
    public string PseudoRowKey { get; set; }


    [JsonConstructor]
    public TrackedCompany()
    {
        
    }
}