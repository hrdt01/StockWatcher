namespace StockTracker.Models.ApiModels;

public interface IRequestContract
{

    /// <summary>
    /// Identifier of the symbol ticker
    /// </summary>
    public string Symbol { get; set; }
}