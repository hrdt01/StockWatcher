using StockTracker.Models.ApiModels.Contracts.Definition;

namespace StockTracker.Models.ApiModels
{
    public class SymbolRangeRequest: ICachedQuery<StockInfoResponse>, IRequestContract
    {
        public string Symbol { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CacheKey => $"{Symbol}_{From.Replace("-",string.Empty)}_{To.Replace("-", string.Empty)}";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(15);
    }
}
