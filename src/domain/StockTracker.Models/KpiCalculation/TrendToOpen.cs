namespace StockTracker.Models.KpiCalculation;

public class TrendToOpen : KpiFigure
{
    public string Id => "ToOpen";
    public new string Explanation => "Evalua la tendencia entre un Open y el Open anterior. Expresado en %";
    public TrendToOpen(string symbol) : base(symbol)
    {
        ComposedId = $"{BaseId}{Id}";
    }

    public override decimal Calculate(decimal? todayValue, decimal? yesterdayValue)
    {
        return ((todayValue.Value * 100) / yesterdayValue.Value) - 100;
    }
}