using FluentValidation;
using StockTracker.ExtractorFunction.Application.Features.Validators;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetDatesBySymbol;

public class GetKpiDatesBySymbolValidator: AbstractValidator<KpiDatesBySymbolRequest>
{
    public GetKpiDatesBySymbolValidator()
    {
        Include(new SymbolRequestValidator<KpiDatesBySymbolRequest>());
    }
}