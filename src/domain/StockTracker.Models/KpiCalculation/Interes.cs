namespace StockTracker.Models.KpiCalculation;

public class Interes : KpiFigure
{
    public new string Id => "Interes";
    public new string Explanation => "Evalua la tendencia entre un Open y el Open anterior.";

    public Interes(string symbol) : base(symbol)
    {
        ComposedId = $"{BaseId}{Id}";
    }

    public override decimal Calculate(decimal? todayValue, decimal? yesterdayValue)
    {
        return todayValue.Value - yesterdayValue.Value;
    }
}