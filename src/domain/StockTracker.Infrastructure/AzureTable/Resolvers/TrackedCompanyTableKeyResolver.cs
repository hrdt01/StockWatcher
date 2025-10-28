using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Resolvers;

public class TrackedCompanyTableKeyResolver : IAzureTableEntityResolver<TrackedCompanyStorageTableKey>
{
    /// <summary>
    /// Returns the string containing the partition key for the entity.
    /// </summary>
    /// <param name="key">Class that contains property partition key.</param>
    /// <returns>String partition key</returns>
    public string ResolvePartitionKey(TrackedCompanyStorageTableKey key)
    {
        return key.Symbol;
    }

    /// <summary>
    /// Returns the string containing the row key for the entity.
    /// </summary>
    /// <param name="key">Class that contains property row key.</param>
    /// <returns>String row key</returns>
    public string ResolveRowKey(TrackedCompanyStorageTableKey key)
    {
        return key.PseudoRowKey;
    }
}