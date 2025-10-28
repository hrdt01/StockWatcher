namespace StockTracker.Models.Persistence;

public class StockInfoModel
{
    public string Symbol { get; set; }
    public string When { get; set; }
    public decimal? Open { get; set; }
    public decimal? Close { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public string ExtractorServiceName { get; set; }
}