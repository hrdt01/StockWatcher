using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface IStockKpiRepository : IRepository<StockKpiModel, StockKpiStorageTableKey>
{
    Task<IEnumerable<string>> GetKpisBySymbolAsync(string symbol);
    Task<IEnumerable<StockKpiModel>> GetKpiInfoByDateRange(string kpiSymbol, string from, string to);
    Task<bool> RemoveEntriesOlderThan(DateTime sourceDate);
}