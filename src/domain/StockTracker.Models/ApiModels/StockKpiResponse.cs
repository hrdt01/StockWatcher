namespace StockTracker.Models.ApiModels;

public class StockKpiResponse
{
    public string SymbolKpi { get; }
    public IEnumerable<KpiValue> KpiValues { get; set; }

    public StockKpiResponse(string symbolKpi)
    {
        SymbolKpi = symbolKpi;
        KpiValues = new List<KpiValue>();
    }
}

public class KpiValue
{
    public string When { get; set; }
    public decimal? Result { get; set; }

    public KpiValue(string when, decimal result)
    {
        When = when;
        Result = result;
    }
}