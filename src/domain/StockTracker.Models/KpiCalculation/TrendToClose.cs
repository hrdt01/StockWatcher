namespace StockTracker.Models.KpiCalculation;

public class TrendToClose: KpiFigure
{
    public new string Id => "ToClose";
    public new string Explanation => "Evalua la tendencia entre un Close y el Close anterior. Expresado en %";
    public TrendToClose(string symbol) : base(symbol)
    {
        ComposedId = $"{BaseId}{Id}";
    }

    public override decimal Calculate(decimal? todayValue, decimal? yesterdayValue)
    {
        return ((todayValue.Value * 100) / yesterdayValue.Value) - 100;
    }
}