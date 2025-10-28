using Microsoft.Extensions.Options;
using StockTracker.CrossCutting.Constants;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class KpiProcessorMessageBroker:
    AzureQueueRepository<KpiProcessMessageRequest>,
    IKpiProcessorMessageBroker
{
    public override string QueueName => GlobalConstants.KpiProcessorQueueName;

    public KpiProcessorMessageBroker(IOptionsMonitor<AzureQueueOptions> options) : base(options)
    {
    }

    public async Task<bool> CreateMessageRequestAsync(KpiProcessMessageRequest message)
    {
        return await base.InsertMessageAsync(message);
    }
}