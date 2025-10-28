using FluentValidation;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.SaveTrackedCompany;

public class SaveTrackedCompanyValidator:AbstractValidator<SaveTrackedCompanyRequest>
{
    public SaveTrackedCompanyValidator()
    {
        RuleFor(request => request.Symbol).NotEmpty();
        RuleFor(request => request.PseudoRowKey).NotEmpty();
        RuleFor(request => request.Name).NotEmpty();
        RuleFor(request => request.Enabled).NotNull();
    }
}