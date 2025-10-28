namespace StockTracker.Models.KpiCalculation;

public class Debilidad : KpiFigure
{
    public new string Id => "Debilidad";
    public new string Explanation => "Evalua la tendencia entre un Close y el Low. Expresado en %";
    public Debilidad(string symbol) : base(symbol)
    {
        ComposedId = $"{BaseId}{Id}";
    }

    public override decimal Calculate(decimal? todayValue, decimal? yesterdayValue)
    {
        return ((todayValue.Value - yesterdayValue.Value) * 100) / todayValue.Value;
    }
}