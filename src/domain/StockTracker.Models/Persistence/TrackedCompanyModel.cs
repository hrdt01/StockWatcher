namespace StockTracker.Models.ApiModels;

public class TrackedCompanyModel
{
    public string Symbol { get; set; }

    public string PseudoRowKey { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }

    public bool Enabled { get; set; }
}