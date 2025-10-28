using Microsoft.Extensions.Options;
using StockTracker.CrossCutting.Constants;
using StockTracker.CrossCutting.Utils;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class StockInfoRepository : 
    AzureTableRepository<StockInfoModel, StockInfoStorageTableKey, StockInfoStorageEntity>, 
    IStockInfoRepository
{
    public StockInfoRepository(
        IOptionsMonitor<AzureTableOptions> options, 
        IAzureTableEntityResolver<StockInfoStorageTableKey> entityResolver)
    : base(options, entityResolver)
    {
    }

    public override string TableName => GlobalConstants.MarketTrackerEndOfDayInfoTableName;
    public async Task<bool> ExistsAsync(StockInfoModel entity)
    {
        var tableKey = new StockInfoStorageTableKey {When = entity.When, Symbol = entity.Symbol};
        return await base.ExistsAsync(tableKey);
    }

    public async Task<IEnumerable<string>> GetAllSymbolsAsync(string searchPattern)
    {
        return await GetAllPartitionsAsync(searchPattern);
    }

    public async Task<IEnumerable<string>> GetDatesBySymbolAsync(string symbol)
    {
        return await GetRowKeysByPartitionKeyAsync(symbol);
    }

    public async Task<IEnumerable<StockInfoModel>> GetStockInfoByDateRange(string symbol, string from, string to)
    {
        var result = new List<StockInfoModel>();
        var arrDates = DateTimeUtils.GetDatesInRange(from, to);
        foreach (var date in arrDates)
        {
            var when = date.ToRowKeyFormat();
            var tableKey = new StockInfoStorageTableKey
            {
                When = when,
                Symbol = symbol
            };
            var exist = await ExistsAsync(tableKey);
            if (!exist) continue;
            
            var entity =
                await GetFromPartitionRowAsync(symbol, when);

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

    protected override StockInfoModel MapFromAzureTableEntity(StockInfoStorageEntity azureTableEntity)
    {
        return new StockInfoModel()
        {
            Symbol = azureTableEntity.PartitionKey,
            When = azureTableEntity.RowKey,
            ExtractorServiceName = azureTableEntity.ExtractorServiceName,
            Close = Convert.ToDecimal(azureTableEntity.Close),
            High = Convert.ToDecimal(azureTableEntity.High),
            Low = Convert.ToDecimal(azureTableEntity.Low),
            Open = Convert.ToDecimal(azureTableEntity.Open)
        };
    }

    protected override StockInfoStorageEntity MapFromEntity(StockInfoModel entity)
    {
        return new StockInfoStorageEntity(entity.Symbol, entity.When)
        {
            Open = Convert.ToDouble(entity.Open),
            High = Convert.ToDouble(entity.High),
            Low = Convert.ToDouble(entity.Low),
            Close = Convert.ToDouble(entity.Close),
            ExtractorServiceName = entity.ExtractorServiceName
        };
    }
}