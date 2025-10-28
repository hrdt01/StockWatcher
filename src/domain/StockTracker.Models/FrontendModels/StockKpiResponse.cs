using System.Text.Json.Serialization;

namespace StockTracker.Models.FrontendModels;

public class StockKpiResponse
{
    [JsonPropertyName(name: "SymbolKpi")]
    public string SymbolKpi { get; set; }

    [JsonPropertyName(name: "KpiValues")]
    public IEnumerable<KpiValue> KpiValues { get; set; }

    public StockKpiResponse(string symbolKpi)
    {
        SymbolKpi = symbolKpi;
        KpiValues = new List<KpiValue>();
    }

    [JsonConstructor]
    public StockKpiResponse()
    { }
}

public class KpiValue
{
    [JsonPropertyName(name: "When")]
    public string When { get; set; }
    [JsonPropertyName(name: "Result")]
    public decimal? Result { get; set; }

    public KpiValue(string when, decimal result)
    {
        When = when;
        Result = result;
    }

    [JsonConstructor]
    public KpiValue()
    { }
}