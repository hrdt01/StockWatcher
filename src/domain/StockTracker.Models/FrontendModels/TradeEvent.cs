using System.Text.Json.Serialization;

namespace StockTracker.Models.FrontendModels;

public class TradeEvent
{
    [JsonPropertyName(name: "Open")]
    public double? Open { get; set; }

    [JsonPropertyName(name: "Close")]
    public double? Close { get; set; }

    [JsonPropertyName(name: "High")]
    public double? High { get; set; }

    [JsonPropertyName(name: "Low")]
    public double? Low { get; set; }

    [JsonPropertyName(name: "When")]
    public DateTime When { get; set; }
}