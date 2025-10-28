using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetInfoBySymbolDateRange;

public class GetInfoBySymbolDateRangeHandler : IRequestHandler<SymbolRangeRequest,StockInfoResponse>
{
    private readonly IStockTracker _stockTracker;
    private readonly ILogger<GetInfoBySymbolDateRangeHandler> _logger;

    public GetInfoBySymbolDateRangeHandler(
        IStockTracker stockTracker,
        ILogger<GetInfoBySymbolDateRangeHandler> logger)
    {
        _stockTracker = stockTracker;
        _logger = logger;
    }
    public async Task<StockInfoResponse> Handle(SymbolRangeRequest request, CancellationToken cancellationToken)
    {
        var result = new StockInfoResponse(request.Symbol);

        result = await _stockTracker.GetStockInfoByDateRange(request.Symbol, request.From, request.To);

        return result;
    }
}