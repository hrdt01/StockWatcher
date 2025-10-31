using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.KpisBySymbolDateRange;

public class KpisBySymbolDateRangeHandler : IRequestHandler<KpiCalculationRequest,bool>
{
    private readonly IStockKpiCalculator _kpiCalculator;
    private readonly IStockTracker _stockTracker;
    private readonly ILogger<KpisBySymbolDateRangeHandler> _logger;

    public KpisBySymbolDateRangeHandler(
        IStockKpiCalculator kpiCalculator,
        IStockTracker stockTracker,
        ILogger<KpisBySymbolDateRangeHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(kpiCalculator);
        ArgumentNullException.ThrowIfNull(stockTracker);
        ArgumentNullException.ThrowIfNull(logger);
        _kpiCalculator = kpiCalculator;
        _stockTracker = stockTracker;
        _logger = logger;
    }
    public async Task<bool> Handle(KpiCalculationRequest request, CancellationToken cancellationToken)
    {
        var previousDate = string.Empty;
        var stacked = new Stack<string>(
            (await _stockTracker.GetDatesBySymbolAsync(request.Symbol)).OrderBy(s => s)
        );
        while (true)
        {
            if (stacked.Count == 0)
            {
                break;
            }

            if (!stacked.Pop().Equals(request.CurrentDate, StringComparison.InvariantCultureIgnoreCase)) 
                continue;
            previousDate = stacked.Pop();
            break;
        }

        if (string.IsNullOrWhiteSpace(previousDate))
            return false;
        var result = await _kpiCalculator.CalculateKpis(request.Symbol, request.CurrentDate, previousDate);

        return result;
    }
}