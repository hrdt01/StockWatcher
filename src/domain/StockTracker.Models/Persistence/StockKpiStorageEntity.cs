using Azure;
using Azure.Data.Tables;

namespace StockTracker.Models.Persistence;

public class StockKpiStorageEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public double? Result { get; set; }

    public StockKpiStorageEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }

    public StockKpiStorageEntity()
    {
        
    }   
}