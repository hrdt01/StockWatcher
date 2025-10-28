using FluentValidation;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetTrackedCompanies;

public class GetTrackedCompaniesValidator : AbstractValidator<TrackedCompaniesRequest>
{
    public GetTrackedCompaniesValidator()
    {
        RuleFor(request => request.Enabled).NotNull();
    }
}