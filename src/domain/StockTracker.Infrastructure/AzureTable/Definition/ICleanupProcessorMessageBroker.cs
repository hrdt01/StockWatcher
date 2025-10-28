using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface ICleanupProcessorMessageBroker
{
    Task<bool> CreateMessageRequestAsync(CleanupProcessMessageRequest message);
}