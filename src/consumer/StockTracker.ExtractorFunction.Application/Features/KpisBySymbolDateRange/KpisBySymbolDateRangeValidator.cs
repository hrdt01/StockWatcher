using FluentValidation;
using StockTracker.CrossCutting.Utils;
using StockTracker.ExtractorFunction.Application.Features.Validators;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.KpisBySymbolDateRange;

public class KpisBySymbolDateRangeValidator : AbstractValidator<KpiCalculationRequest>
{
    public KpisBySymbolDateRangeValidator()
    {
        Include(new SymbolRequestValidator<KpiCalculationRequest>());
        RuleFor(request => request.CurrentDate)
            .NotEmpty()
            .Must(value => DateTime.TryParse(value, out DateTime dateTimeFrom))
            .WithMessage("Date is wrong.");
        RuleFor(request => request).Must(args => DateTime.Parse(args.CurrentDate).ReadyToKpiCalculation())
            .WithMessage("Incorrect Date to calculate Kpis.");
    }   
}