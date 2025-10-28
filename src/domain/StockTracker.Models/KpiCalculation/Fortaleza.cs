namespace StockTracker.Models.KpiCalculation;

public class Fortaleza : KpiFigure
{
    public new string Id => "Fortaleza";
    public new string Explanation => "Evalua la tendencia entre un High y el Close. Expresado en %";

    public Fortaleza(string symbol) : base(symbol)
    {
        ComposedId = $"{BaseId}{Id}";
    }

    public override decimal Calculate(decimal? todayValue, decimal? yesterdayValue)
    {
        return ((todayValue.Value - yesterdayValue.Value) * 100) / yesterdayValue.Value;
    }
}