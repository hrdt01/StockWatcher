namespace StockTracker.Models;

public class StockInfo
{
    public string TickerSymbol { get; }
    public List<TradeEvent> TradeEvents { get; set; }

    public StockInfo(string symbol)
    {
        TickerSymbol = symbol;
        TradeEvents = new List<TradeEvent>();
    }
}