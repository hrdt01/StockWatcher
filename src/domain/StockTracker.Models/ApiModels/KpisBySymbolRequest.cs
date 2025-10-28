using StockTracker.Models.ApiModels.Contracts.Definition;
using System.Collections.ObjectModel;

namespace StockTracker.Models.ApiModels;

public class KpisBySymbolRequest : ICachedQuery<ReadOnlyDictionary<string, string>>, IRequestContract
{
    public string Symbol { get; set; }
    public string CacheKey => $"{nameof(KpisBySymbolRequest)}_{Symbol}";
    public TimeSpan? Expiration => null;
}