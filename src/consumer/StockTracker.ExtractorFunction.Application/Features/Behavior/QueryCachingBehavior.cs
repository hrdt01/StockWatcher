using MediatR;
using StockTracker.ExtractorFunction.Application.Contracts.Definition;
using StockTracker.Models.ApiModels.Contracts.Definition;

namespace StockTracker.ExtractorFunction.Application.Features.Behavior;


/// <summary>
/// Class to centralize cache operations performed by MediatR handlers
/// </summary>
/// <typeparam name="TRequest">Request processed by MediatR handlers</typeparam>
/// <typeparam name="TResponse">Response offered by MediatR handlers</typeparam>
internal sealed class QueryCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
{
    private readonly ICacheService _cacheService;

    public QueryCachingBehavior(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }


    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await _cacheService.GetOrCreateAsync(
            request.CacheKey,
            _ => next(),
            request.Expiration,
            cancellationToken);
    }
}