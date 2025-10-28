using FluentValidation;
using StockTracker.ExtractorFunction.Application.Features.Validators;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetKpisBySymbol;

public class GetKpisBySymbolValidator : AbstractValidator<KpisBySymbolRequest>
{
    public GetKpisBySymbolValidator()
    {
        Include(new SymbolRequestValidator<KpisBySymbolRequest>());
    }
    
}