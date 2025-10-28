using Microsoft.Extensions.Options;
using StockTracker.CrossCutting.Constants;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class CleanupProcessorMessageBroker :
    AzureQueueRepository<CleanupProcessMessageRequest>,
    ICleanupProcessorMessageBroker
{
    public override string QueueName => GlobalConstants.CleanupProcessorQueueName;

    public CleanupProcessorMessageBroker(IOptionsMonitor<AzureQueueOptions> options) : base(options)
    {
    }

    public async Task<bool> CreateMessageRequestAsync(CleanupProcessMessageRequest message)
    {
        return await base.InsertMessageAsync(message);
    }
}