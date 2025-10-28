using Azure;
using Azure.Data.Tables;

namespace StockTracker.Models.Persistence;

public class StockInfoStorageEntity: ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public double? Open { get; set; }
    public double? Close { get; set; }
    public double? High { get; set; }
    public double? Low { get; set; }
    public string ExtractorServiceName { get; set; }

    public StockInfoStorageEntity(string partitionKey, string rowKey)
    {
        PartitionKey = partitionKey;
        RowKey = rowKey;
    }

    public StockInfoStorageEntity()
    {
        
    }   
}