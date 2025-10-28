namespace StockTracker.MarketStack.Services.Models;

public class EndOfDayResponse
{
    public Pagination pagination { get; set; }
    public IEnumerable<EndOfDaySymbol> data { get; set; }
}