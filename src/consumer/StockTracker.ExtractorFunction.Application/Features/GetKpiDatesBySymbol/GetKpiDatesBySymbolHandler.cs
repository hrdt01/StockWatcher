using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetDatesBySymbol;

public class GetKpiDatesBySymbolHandler : IRequestHandler<KpiDatesBySymbolRequest, IEnumerable<string>>
{
    private readonly IStockKpiCalculator _stockKpiCalculator;
    private readonly ILogger<GetKpiDatesBySymbolHandler> _logger;

    public GetKpiDatesBySymbolHandler(
        IStockKpiCalculator stockKpiCalculator,
        ILogger<GetKpiDatesBySymbolHandler> logger)
    {
        _stockKpiCalculator = stockKpiCalculator;
        _logger = logger;
    }
    public async Task<IEnumerable<string>> Handle(KpiDatesBySymbolRequest request, CancellationToken cancellationToken)
    {
        return await _stockKpiCalculator.GetKpiDatesBySymbol(request.Symbol);
    }
}