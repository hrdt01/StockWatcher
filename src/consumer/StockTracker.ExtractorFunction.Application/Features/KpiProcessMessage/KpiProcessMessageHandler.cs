using MediatR;
using Microsoft.Extensions.Logging;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction.Application.Features.KpiProcessMessage;

public class KpiProcessMessageHandler : IRequestHandler<KpiProcessMessageRequest, bool>
{
    private readonly IKpiProcessorMessageBroker _kpiProcessorMessageBroker;
    private readonly ILogger<KpiProcessMessageHandler> _logger;

    public KpiProcessMessageHandler(
        IKpiProcessorMessageBroker kpiProcessorMessageBroker,
        ILogger<KpiProcessMessageHandler> logger)
    {
        _kpiProcessorMessageBroker = kpiProcessorMessageBroker;
        _logger = logger;
    }

    public async Task<bool> Handle(KpiProcessMessageRequest request, CancellationToken cancellationToken)
    {
        var result = await _kpiProcessorMessageBroker.CreateMessageRequestAsync(request);

        return result;
    }
}