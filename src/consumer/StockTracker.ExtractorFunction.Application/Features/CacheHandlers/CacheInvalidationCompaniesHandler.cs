using MediatR;
using StockTracker.ExtractorFunction.Application.Contracts.Definition;
using StockTracker.ExtractorFunction.Application.Features.SaveTrackedCompany;

namespace StockTracker.ExtractorFunction.Application.Features.CacheHandlers;

internal class CacheInvalidationCompaniesHandler :
    INotificationHandler<SaveTrackedCompanyEvent>
{
    private readonly ICacheService _cacheService;

    public CacheInvalidationCompaniesHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(SaveTrackedCompanyEvent notification, CancellationToken cancellationToken)
    {
        foreach (var cacheKey in notification.AssociatedCacheKeys)
        {
            await HandleInternal(cacheKey, cancellationToken);
        }
    }

    private Task HandleInternal(string cacheKey, CancellationToken cancellationToken)
    {
        _cacheService.RemoveFromCache(cacheKey, cancellationToken);
        return Task.CompletedTask;
    }
}