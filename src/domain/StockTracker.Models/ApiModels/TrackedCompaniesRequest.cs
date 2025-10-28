using StockTracker.CrossCutting.Constants;
using StockTracker.Models.ApiModels.Contracts.Definition;

namespace StockTracker.Models.ApiModels;

public class TrackedCompaniesRequest : ICachedQuery<IEnumerable<TrackedCompaniesResponse>>
{
    public bool Enabled { get; set; }
    public string CacheKey => 
        Enabled ? ApiConstants.GetTrackedCompaniesFilteredCacheKey : ApiConstants.GetTrackedCompaniesAllCacheKey;
    public TimeSpan? Expiration => null;
}