using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.GetTrackedCompanies;

public class GetTrackedCompaniesHandler : IRequestHandler<TrackedCompaniesRequest, IEnumerable<TrackedCompaniesResponse>>
{
    private readonly IStockTracker _stockTracker;
    private readonly ILogger<GetTrackedCompaniesHandler> _logger;

    public GetTrackedCompaniesHandler(
        IStockTracker stockTracker,
        ILogger<GetTrackedCompaniesHandler> logger)
    {
        _stockTracker = stockTracker;
        _logger = logger;
    }
    public async Task<IEnumerable<TrackedCompaniesResponse>> Handle(TrackedCompaniesRequest request, CancellationToken cancellationToken)
    {
        var fromPersistence = await _stockTracker.GetTrackedCompanies(request.Enabled);
        
        var result = fromPersistence.Select(
            model => new TrackedCompaniesResponse()
            {
                Enabled = model.Enabled,
                Symbol = model.Symbol,
                Url = model.Url,
                Name = model.Name,
                PseudoRowKey = model.PseudoRowKey
            }
        );
        return result;
    }
}