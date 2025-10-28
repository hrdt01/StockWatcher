using StockTracker.MarketStack.Services.Models;
using StockTracker.Models.ApiModels;

namespace StockTracker.MarketStack.Services.Contracts.Definition;

public interface IStockTracker
{
    /// <summary>
    /// Gets the end of day info for a given <paramref name="tickerSymbol"/> and a given <paramref name="targetDay"/>
    /// </summary>
    /// <param name="tickerSymbol">Stock symbol</param>
    /// <param name="targetDay">Date in YYYY-MM-DD format or in ISO-8601 date format, e.g. 2020-05-21T00:00:00+0000</param>
    /// <returns>Instance of <see cref="EndOfDayResponse"/></returns>
    ValueTask<EndOfDayResponse?> GetEndOfDayBySymbol(string tickerSymbol, DateTime targetDay);

    /// <summary>
    /// Consolidate data retrieved for a specific end of day request based on a given <paramref name="tickerSymbol"/> and a given <paramref name="targetDay"/>
    /// </summary>
    /// <param name="sourceData">Instance of <see cref="EndOfDayResponse"/></param>
    /// <param name="tickerSymbol">Stock symbol</param>
    /// <param name="targetDay">Date in YYYY-MM-DD format</param>
    /// <returns></returns>
    ValueTask<bool> ConsolidateEndOfDayInfo(EndOfDayResponse sourceData, string tickerSymbol, DateTime targetDay);

    /// <summary>
    /// Build the complete URL to EndOfDay endpoint
    /// </summary>
    /// <param name="accessKey"></param>
    /// <param name="targetDay"></param>
    /// <returns></returns>
    string BuildEndOfDayEndpointRequest(string accessKey, DateTime targetDay);

    /// <summary>
    /// Retrieve all persisted symbols given a search pattern evaluating if table partition key ends with <param name="searchPattern"></param> 
    /// </summary>
    /// <param name="searchPattern">Suffix pattern. Default to: BMEX</param>
    /// <returns>Collection of symbol tickers</returns>
    ValueTask<IEnumerable<string>> GetAllSymbolsAsync(string searchPattern = "BMEX");

    /// <summary>
    /// Get all registered dates given a <param name="symbol"></param>
    /// </summary>
    /// <param name="symbol">Symbol ticker</param>
    /// <returns>Collection of registered dates</returns>
    ValueTask<IEnumerable<string>> GetDatesBySymbolAsync(string symbol);

    /// <summary>
    /// Retrieve all persisted endofday information given a <param name="symbol"></param>
    /// and a date range defined by <param name="from"></param> and <param name="to"></param> 
    /// </summary>
    /// <param name="symbol">Symbol ticker</param>
    /// <param name="from">Initial date</param>
    /// <param name="to">End date</param>
    /// <returns>Instance of <see cref="StockInfoResponse"/></returns>
    ValueTask<StockInfoResponse> GetStockInfoByDateRange(string symbol, string from, string to);

    /// <summary>
    /// Get all companies that are being tracked
    /// If <param name="enabled"></param> is true, filter them by the enabled property
    /// If <param name="enabled"></param> is false, return them all.
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns>Collection of <see cref="TrackedCompanyModel"/></returns>
    ValueTask<IEnumerable<TrackedCompanyModel>> GetTrackedCompanies(bool enabled = false);

    ValueTask<bool> ExecuteTrackedCompaniesInitialMigration();

    /// <summary>
    /// Persist the entity supplied 
    /// </summary>
    /// <param name="source">Source entity <see cref="TrackedCompanyModel"/></param>
    /// <returns></returns>
    Task<bool> SaveTrackedCompany(TrackedCompanyModel source);

    /// <summary>
    /// Check if a company already exist to evaluate if it's the first time the company is created
    /// </summary>
    /// <param name="symbol">Company's ticker symbol</param>
    /// <returns></returns>
    Task<bool> CheckExistingCompany(string symbol);
}