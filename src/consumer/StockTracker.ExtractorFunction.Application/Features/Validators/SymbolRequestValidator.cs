using FluentValidation;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.Validators;

internal class SymbolRequestValidator<TRequest> : AbstractValidator<TRequest> where TRequest : class, IRequestContract
{
    public SymbolRequestValidator()
    {
        RuleFor(request => request.Symbol).NotEmpty();
    }
}