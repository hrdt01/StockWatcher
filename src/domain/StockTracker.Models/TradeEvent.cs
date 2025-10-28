namespace StockTracker.Models;

public class TradeEvent
{
    public double? Open { get; set; }
    public double? Close { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }
    public DateTime When { get; set; }
}