using Azure;
using Azure.Data.Tables;

namespace StockTracker.Models.Persistence;

public class TrackedCompanyStorageEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Name { get; set; }

    public string Url { get; set; }

    public bool Enabled { get; set; }

    public TrackedCompanyStorageEntity()
    {
    }

    public TrackedCompanyStorageEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }
}