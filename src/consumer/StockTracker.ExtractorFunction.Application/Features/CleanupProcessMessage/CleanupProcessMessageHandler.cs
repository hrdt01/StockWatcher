using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.KpiProcessMessage;

public class CleanupProcessMessageHandler : IRequestHandler<CleanupProcessMessageRequest, bool>
{
    private readonly ICleanupProcessorMessageBroker _cleanupProcessorMessageBroker;
    private readonly ILogger<CleanupProcessMessageHandler> _logger;

    public CleanupProcessMessageHandler(
        ICleanupProcessorMessageBroker cleanupProcessorMessageBroker,
        ILogger<CleanupProcessMessageHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(cleanupProcessorMessageBroker);
        ArgumentNullException.ThrowIfNull(logger);
        _cleanupProcessorMessageBroker = cleanupProcessorMessageBroker;
        _logger = logger;
    }

    public async Task<bool> Handle(CleanupProcessMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await _cleanupProcessorMessageBroker.CreateMessageRequestAsync(request);

        return result;
    }
}