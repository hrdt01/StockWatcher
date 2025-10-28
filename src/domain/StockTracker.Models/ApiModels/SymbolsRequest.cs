using StockTracker.Models.ApiModels.Contracts.Definition;

namespace StockTracker.Models.ApiModels;

public class SymbolsRequest: ICachedQuery<IEnumerable<string>>
{
    public string CacheKey => $"{nameof(SymbolsRequest)}";
    public TimeSpan? Expiration => null;
}