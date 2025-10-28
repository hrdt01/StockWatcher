using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;
using System.Collections.ObjectModel;

namespace StockTracker.ExtractorFunction.Application.Features.GetKpisBySymbol;

public class GetKpisBySymbolHandler : IRequestHandler<KpisBySymbolRequest, ReadOnlyDictionary<string, string>>
{
    private readonly IStockKpiCalculator _kpiCalculator;
    private readonly ILogger<GetKpisBySymbolHandler> _logger;

    public GetKpisBySymbolHandler(
        IStockKpiCalculator kpiCalculator,
        ILogger<GetKpisBySymbolHandler> logger)
    {
        _kpiCalculator = kpiCalculator;
        _logger = logger;
    }

    public async Task<ReadOnlyDictionary<string, string>> Handle(KpisBySymbolRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(GetKpisBySymbolHandler)}: Retrieving persisted kpis for: {request.Symbol}");
        return await _kpiCalculator.GetPersistedKpiNamesBySymbol(request.Symbol);
    }
}