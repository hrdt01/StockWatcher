namespace StockTracker.Models.Persistence;

public class StockKpiModel
{
    public string SymbolKpiId { get; set; }
    public string When { get; set; }
    public decimal? Result { get; set; }
}