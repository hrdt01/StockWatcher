using MediatR;
using StockTracker.CrossCutting.Constants;

namespace StockTracker.ExtractorFunction.Application.Features.SaveTrackedCompany;

public record SaveTrackedCompanyEvent() : INotification
{
    public string[] AssociatedCacheKeys { get; init; } =
        [
            ApiConstants.GetTrackedCompaniesAllCacheKey, 
            ApiConstants.GetTrackedCompaniesFilteredCacheKey
        ];
}