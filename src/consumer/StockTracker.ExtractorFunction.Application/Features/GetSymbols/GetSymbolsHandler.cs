using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.MarketStack.Services.Contracts.Implementation;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetSymbols;

public class GetSymbolsHandler : IRequestHandler<SymbolsRequest, IEnumerable<string>>
{
    private readonly ILogger<GetSymbolsHandler> _logger;
    private readonly IStockTracker _stockTracker;

    public GetSymbolsHandler(
        IStockTracker stockTracker,
        ILogger<GetSymbolsHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(stockTracker);
        ArgumentNullException.ThrowIfNull(logger);
        _stockTracker = stockTracker;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> Handle(SymbolsRequest request, CancellationToken cancellationToken)
    {
        return await _stockTracker.GetAllSymbolsAsync();
    }
}