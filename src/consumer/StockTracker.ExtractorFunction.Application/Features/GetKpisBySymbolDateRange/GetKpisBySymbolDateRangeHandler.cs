using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetKpisBySymbolDateRange;

public class GetKpisBySymbolDateRangeHandler : IRequestHandler<KpisBySymbolDateRangeRequest, StockKpiResponse>
{
    private readonly IStockKpiCalculator _stockKpiCalculator;
    private readonly ILogger<GetKpisBySymbolDateRangeHandler> _logger;

    public GetKpisBySymbolDateRangeHandler(
        IStockKpiCalculator stockKpiCalculator,
        ILogger<GetKpisBySymbolDateRangeHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(stockKpiCalculator);
        ArgumentNullException.ThrowIfNull(logger);
        _stockKpiCalculator = stockKpiCalculator;
        _logger = logger;
    }
    public async Task<StockKpiResponse> Handle(KpisBySymbolDateRangeRequest request, CancellationToken cancellationToken)
    {
        var kpiSymbol = request.SymbolKpi;
        var result = new StockKpiResponse(kpiSymbol);

        var persistedKpis =
            await _stockKpiCalculator.GetPersistedKpiBySymbolByDateRange(kpiSymbol, request.From, request.To);

        result.KpiValues = persistedKpis.Select(model => new KpiValue(model.When, model.Result!.Value));

        return result;
    }
}