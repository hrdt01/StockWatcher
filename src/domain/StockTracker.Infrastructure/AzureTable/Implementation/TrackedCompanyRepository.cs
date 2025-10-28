using Microsoft.Extensions.Options;
using StockTracker.CrossCutting.Constants;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class TrackedCompanyRepository :
    AzureTableRepository<TrackedCompanyModel, TrackedCompanyStorageTableKey, TrackedCompanyStorageEntity>,
    ITrackedCompanyRepository
{
    public TrackedCompanyRepository(
        IOptionsMonitor<AzureTableOptions> options,
        IAzureTableEntityResolver<TrackedCompanyStorageTableKey> entityResolver) : base(options, entityResolver)
    {
        
    }

    public override string TableName => GlobalConstants.TrackedCompanyTableName;
    
    protected override TrackedCompanyModel MapFromAzureTableEntity(TrackedCompanyStorageEntity azureTableEntity)
    {
        return new TrackedCompanyModel()
        {
            Symbol = azureTableEntity.PartitionKey,
            PseudoRowKey = azureTableEntity.RowKey,
            Enabled = azureTableEntity.Enabled,
            Name = azureTableEntity.Name,
            Url = azureTableEntity.Url
        };
    }

    protected override TrackedCompanyStorageEntity MapFromEntity(TrackedCompanyModel entity)
    {
        return new TrackedCompanyStorageEntity(entity.Symbol, entity.PseudoRowKey)
        {
            Name= entity.Name,
            Url = entity.Url,
            Enabled = entity.Enabled
        };
    }

    public async Task<IEnumerable<TrackedCompanyModel>> GetTrackedCompaniesAsync(bool enabled = false)
    {
        var result = new List<TrackedCompanyModel>();
        var allPartitions = await GetAllPartitionsAsync(string.Empty);
        foreach (var symbol in allPartitions)
        {
            var allRowKeysByPartition = await GetRowKeysByPartitionKeyAsync(symbol);
            foreach (var rowKey in allRowKeysByPartition)
            {
                var trackedCompany = await GetFromPartitionRowAsync(symbol, rowKey);
                result.Add(trackedCompany);
            }
        }

        return enabled ? result.Where(model => model.Enabled) : result;
    }
}