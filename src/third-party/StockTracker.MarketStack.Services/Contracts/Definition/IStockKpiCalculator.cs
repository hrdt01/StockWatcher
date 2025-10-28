using System.Collections.ObjectModel;
using StockTracker.Models.Persistence;

namespace StockTracker.MarketStack.Services.Contracts.Definition;

public interface IStockKpiCalculator
{
    /// <summary>
    /// Calculate a collection KPIs for a given <param name="symbol"></param>
    /// based on current date defined by <param name="currentDate"></param> and <param name="previousDate"></param> 
    /// </summary>
    /// <param name="symbol">Symbol ticker</param>
    /// <param name="currentDate">Current date</param>
    /// <param name="previousDate">Previous date</param>
    ValueTask<bool> CalculateKpis(string symbol, string currentDate, string previousDate);

    /// <summary>
    /// Retrieve the list of kpi names registered by supplied <param name="symbol"></param>
    /// </summary>
    /// <param name="symbol">Symbol ticker</param>
    /// <returns></returns>
    Task<ReadOnlyDictionary<string, string>> GetPersistedKpiNamesBySymbol(string symbol);

    /// <summary>
    /// Get persisted information of <param name="kpiSymbol"></param>
    /// between <param name="from"></param> and <param name="to"></param>
    /// </summary>
    /// <param name="kpiSymbol">Kpi Symbol identifier</param>
    /// <param name="from">From date</param>
    /// <param name="to">To date</param>
    /// <returns>Collection of <see cref="StockKpiModel"/></returns>
    ValueTask<IEnumerable<StockKpiModel>> GetPersistedKpiBySymbolByDateRange(
        string kpiSymbol, string from, string to);

    /// <summary>
    /// Get the persisted range of dates of the kpis identified by <param name="symbol"></param>
    /// Retrieve all RK where its corresponding PK starts with <param name="symbol"></param>
    /// </summary>
    /// <param name="symbol">Identifier of the ticker symbol</param>
    /// <returns>Collection of dates(aka RK) in "yyyy-MM-dd" format</returns>
    ValueTask<IEnumerable<string>> GetKpiDatesBySymbol(string symbol);

    /// <summary>
    /// Removes from persistent stores the information prior to <param name="sourceDate"></param>
    /// </summary>
    /// <param name="sourceDate">Datetime to evaluate</param>
    /// <returns></returns>
    Task<bool> CleanupDeprecatedInfo(DateTime sourceDate);
}