using System.Text.Json.Serialization;

namespace StockTracker.Frontend.Services.Models;

public class StockInfoResponse
{
    [JsonPropertyName(name: "TickerSymbol")]
    public string TickerSymbol { get; set; }

    [JsonPropertyName(name: "TradeEvents")]
    public List<TradeEvent> TradeEvents { get; set; }

    [JsonConstructor]
    public StockInfoResponse()
    {
    }
}