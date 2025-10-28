namespace StockTracker.ExtractorFunction.Application.Contracts.Definition;
public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    void RemoveFromCache(string cacheKey, CancellationToken cancellationToken = default);
}