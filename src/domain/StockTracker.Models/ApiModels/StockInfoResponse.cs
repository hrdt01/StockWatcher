namespace StockTracker.Models.ApiModels;

public class StockInfoResponse
{
    public string TickerSymbol { get; }
    public List<TradeEvent> TradeEvents { get; set; }


    public StockInfoResponse(string symbol)
    {
        TickerSymbol = symbol;
        TradeEvents = new List<TradeEvent>();
    }
}