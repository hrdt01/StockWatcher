namespace StockTracker.Models.KpiCalculation;

public abstract class KpiFigure
{
    public string BaseId { get; private set; }
    public string Explanation { get; private set; }
    public string ComposedId { get; set; }

    internal KpiFigure(string symbol)
    {
        BaseId = $"{symbol}_";
    }

    public abstract decimal Calculate(decimal? todayValue, decimal? yesterdayValue);
}