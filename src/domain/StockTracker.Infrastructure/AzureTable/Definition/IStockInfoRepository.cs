using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface IStockInfoRepository : IRepository<StockInfoModel, StockInfoStorageTableKey>
{
    Task<bool> ExistsAsync(StockInfoModel entity);
    Task<IEnumerable<string>> GetAllSymbolsAsync(string searchPattern);
    Task<IEnumerable<string>> GetDatesBySymbolAsync(string symbol);
    Task<IEnumerable<StockInfoModel>> GetStockInfoByDateRange(string symbol, string from, string to);
    Task<bool> RemoveEntriesOlderThan(DateTime sourceDate);
}