using System.ComponentModel.DataAnnotations;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class AzureTableOptions
{
    public AzureTableOptions()
    {
        CreateTableIfNotExists = true;
        MaxConnectionLimit = 200;
        BulkOperationLimit = 100;
        MaxParallelBulkOperations = 10;
    }

    [Required]
    public string ConnectionString { get; set; }

    public bool CreateTableIfNotExists { get; set; }
    public int MaxConnectionLimit { get; set; }
    public int BulkOperationLimit { get; set; }
    public int MaxParallelBulkOperations { get; set; }
}