namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface IAzureTableEntityResolver<in TKey>
{
    string ResolvePartitionKey(TKey key);
    string ResolveRowKey(TKey key);
}