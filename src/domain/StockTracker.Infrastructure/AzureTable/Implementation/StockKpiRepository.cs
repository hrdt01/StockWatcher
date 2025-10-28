using Microsoft.Extensions.Options;
using StockTracker.CrossCutting.Constants;
using StockTracker.CrossCutting.Utils;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class StockKpiRepository :
    AzureTableRepository<StockKpiModel, StockKpiStorageTableKey, StockKpiStorageEntity>,
    IStockKpiRepository
{
    public StockKpiRepository(
        IOptionsMonitor<AzureTableOptions> options,
        IAzureTableEntityResolver<StockKpiStorageTableKey> entityResolver) : base(options, entityResolver)
    {
        
    }
    public override string TableName => GlobalConstants.StockKpiTableName;
    
    protected override StockKpiModel MapFromAzureTableEntity(StockKpiStorageEntity azureTableEntity)
    {
        return new StockKpiModel()
        {
            SymbolKpiId = azureTableEntity.PartitionKey,
            When = azureTableEntity.RowKey,
            Result = Convert.ToDecimal(azureTableEntity.Result)
        };
    }

    protected override StockKpiStorageEntity MapFromEntity(StockKpiModel entity)
    {
        return new StockKpiStorageEntity(entity.SymbolKpiId, entity.When)
        {
            Result = Convert.ToDouble(entity.Result)
        };
    }

    public async Task<IEnumerable<string>> GetKpisBySymbolAsync(string symbol)
    {
        return await GetPartitionsByPatternAsync(symbol);
    }

    public async Task<IEnumerable<StockKpiModel>> GetKpiInfoByDateRange(string kpiSymbol, string from, string to)
    {
        var result = new List<StockKpiModel>();
        var arrDates = DateTimeUtils.GetDatesInRange(from, to);
        foreach (var date in arrDates)
        {
            var when = date.ToRowKeyFormat();
            var tableKey = new StockKpiStorageTableKey
            {
                When = when,
                SymbolKpiId = kpiSymbol
            };
            var exist = await ExistsAsync(tableKey);
            if (!exist) continue;

            var entity =
                await GetFromPartitionRowAsync(kpiSymbol, when);

            result.Add(entity);
        }

        return result;
    }

    public async Task<bool> RemoveEntriesOlderThan(DateTime sourceDate)
    {
        var collection = await GetByTimestampAsync(sourceDate);
        var result = await DeleteAsync(collection);
        return result;
    }
}