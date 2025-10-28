using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Resolvers;

public class StockKpiTableKeyResolver : IAzureTableEntityResolver<StockKpiStorageTableKey>
{
    /// <summary>
    /// Returns the string containing the partition key for the entity.
    /// </summary>
    /// <param name="key">Class that contains property partition key.</param>
    /// <returns>String partition key</returns>
    public string ResolvePartitionKey(StockKpiStorageTableKey key)
    {
        return key.SymbolKpiId;
    }

    /// <summary>
    /// Returns the string containing the row key for the entity.
    /// </summary>
    /// <param name="key">Class that contains property row key.</param>
    /// <returns>String row key</returns>
    public string ResolveRowKey(StockKpiStorageTableKey key)
    {
        return key.When;
    }
}