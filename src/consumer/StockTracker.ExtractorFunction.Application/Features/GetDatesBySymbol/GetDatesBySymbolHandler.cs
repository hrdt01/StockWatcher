using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetDatesBySymbol;

public class GetDatesBySymbolHandler : IRequestHandler<DatesBySymbolRequest, IEnumerable<string>>
{
    private readonly IStockTracker _stockTracker;
    private readonly ILogger<GetDatesBySymbolHandler> _logger;

    public GetDatesBySymbolHandler(
        IStockTracker stockTracker,
        ILogger<GetDatesBySymbolHandler> logger)
    {
        _stockTracker = stockTracker;
        _logger = logger;
    }
    public async Task<IEnumerable<string>> Handle(DatesBySymbolRequest request, CancellationToken cancellationToken)
    {
        return await _stockTracker.GetDatesBySymbolAsync(request.Symbol);
    }
}