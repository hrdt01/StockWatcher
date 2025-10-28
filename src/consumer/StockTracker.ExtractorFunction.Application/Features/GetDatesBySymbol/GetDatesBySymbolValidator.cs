using FluentValidation;
using StockTracker.ExtractorFunction.Application.Features.Validators;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetDatesBySymbol;

public class GetDatesBySymbolValidator: AbstractValidator<DatesBySymbolRequest>
{
    public GetDatesBySymbolValidator()
    {
        Include(new SymbolRequestValidator<DatesBySymbolRequest>());
    }
}