namespace StockTracker.Frontend.Services.Models;

public class StockInfoChartModel
{
    public decimal? Open { get; set; }

    public decimal? Close { get; set; }

    public decimal? High { get; set; }
    
    public decimal? Low { get; set; }

    public DateTime When { get; set; }
}

public static partial class Converter
{
    public static StockInfoChartModel ToChartModel(this TradeEvent source)
    {
        return new StockInfoChartModel()
        {
            Close = Convert.ToDecimal(source.Close),
            High = Convert.ToDecimal(source.High),
            Low = Convert.ToDecimal(source.Low),
            Open = Convert.ToDecimal(source.Open),
            When = source.When
        };
    }
    public static StockInfoChartModel[] ToChartModel(this IEnumerable<TradeEvent> source)
    {
        var result = new List<StockInfoChartModel>();
        result.AddRange(source.Select(item => item.ToChartModel()));
        return result.ToArray();
    }
}
