using FluentValidation;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetKpisBySymbolDateRange;

public class GetKpisBySymbolDateRangeValidator : AbstractValidator<KpisBySymbolDateRangeRequest>
{
    public GetKpisBySymbolDateRangeValidator()
    {
        RuleFor(request => request.SymbolKpi).NotEmpty();
        RuleFor(request => request.From)
            .NotEmpty()
            .Must(value => DateTime.TryParse(value, out DateTime dateTimeFrom))
            .WithMessage("Date is wrong."); ;
        RuleFor(request => request.To)
            .NotEmpty()
            .Must(value => DateTime.TryParse(value, out DateTime dateTimeTo))
            .WithMessage("Date is wrong.");
        RuleFor(request => request).Must(args => BeValidRange(args.From, args.To))
            .WithMessage("Date is wrong.");
    }

    /// <summary>
    /// Check <param name="dateFrom"></param> cannot be prior or same date as <param name="dateTo"></param>
    /// </summary>
    /// <param name="dateFrom">String representation of date under this format: "YYYY-MM-DD"</param>
    /// <param name="dateTo">String representation of date under this format: "YYYY-MM-DD"</param>
    /// <returns></returns>
    private bool BeValidRange(string dateFrom, string dateTo)
    {
        var from = DateTime.TryParse(dateFrom, out DateTime dateTimeFrom);
        var to = DateTime.TryParse(dateTo, out DateTime dateTimeTo);
        return from & to && dateTimeFrom < dateTimeTo;
    }
}