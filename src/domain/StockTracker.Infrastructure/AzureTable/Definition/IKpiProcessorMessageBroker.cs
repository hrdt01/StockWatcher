using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface IKpiProcessorMessageBroker
{
    Task<bool> CreateMessageRequestAsync(KpiProcessMessageRequest message);
}