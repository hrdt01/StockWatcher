using System.ComponentModel.DataAnnotations;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class AzureQueueOptions
{
    [Required]
    public string ConnectionString { get; set; }

    public bool CreateQueueIfNotExists { get; set; }
    public AzureQueueOptions()
    {
        CreateQueueIfNotExists = true;
    }

}