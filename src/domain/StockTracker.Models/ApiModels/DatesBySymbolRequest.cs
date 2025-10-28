using StockTracker.Models.ApiModels.Contracts.Definition;

namespace StockTracker.Models.ApiModels;

public class DatesBySymbolRequest : ICachedQuery<IEnumerable<string>>, IRequestContract
{
    public string Symbol { get; set; }
    public string CacheKey => $"{nameof(DatesBySymbolRequest)}_{Symbol}";
    public TimeSpan? Expiration => null;
}