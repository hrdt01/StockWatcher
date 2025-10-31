using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.KpiProcessMessage;

public class CleanupProcessHandler : IRequestHandler<CleanupProcessRequest, bool>
{
    private readonly IStockKpiCalculator _kpiCalculator;
    private readonly ILogger<CleanupProcessHandler> _logger;

    public CleanupProcessHandler(
        IStockKpiCalculator kpiCalculator,
        ILogger<CleanupProcessHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(kpiCalculator);
        ArgumentNullException.ThrowIfNull(logger);

        _kpiCalculator = kpiCalculator;
        _logger = logger;
    }

    public async Task<bool> Handle(CleanupProcessRequest request, CancellationToken cancellationToken)
    {
        var result = await _kpiCalculator.CleanupDeprecatedInfo(request.LimitDate);

        return result;
    }
}