using Microsoft.Extensions.Caching.Memory;
using StockTracker.ExtractorFunction.Application.Contracts.Definition;

namespace StockTracker.ExtractorFunction.Application.Contracts.Implementation;

internal sealed class CacheService : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key, 
        Func<CancellationToken, Task<T>> factory, 
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        T result = await _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);

                return factory(cancellationToken);
            });

        return result;
    }

    public void RemoveFromCache(string cacheKey, CancellationToken cancellationToken = default)
    {
        if(_memoryCache.Get(cacheKey) is not null)
            _memoryCache.Remove(cacheKey);
    }
}