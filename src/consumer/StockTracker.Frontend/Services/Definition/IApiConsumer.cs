using System.Collections.ObjectModel;
using StockTracker.Frontend.Identity.Models;
using StockTracker.Frontend.Services.Models;

namespace StockTracker.Frontend.Services.Definition;

public interface IApiConsumer
{
    /// <param name="tokenModel"></param>
    /// <inheritdoc />
    Task<IEnumerable<string>> GetSymbols(TokenModel? tokenModel);

    /// <inheritdoc />
    Task<IEnumerable<string>> GetDatesBySymbol(string symbol, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<StockInfoResponse> GetInfoBySymbolDateRange(string symbol, string from, string to, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<ReadOnlyDictionary<string, string>> GetKpisBySymbol(string symbol, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<StockKpiResponse> GetKpisBySymbolDateRange(string symbolKpi, string from, string to, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<ReadOnlyCollection<TrackedCompany>> GetTrackedCompanies(bool enabled, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<bool> SaveTrackedCompany(TrackedCompany source, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<IEnumerable<string>> GetKpiDatesBySymbol(string symbol, TokenModel? tokenModel);

    /// <inheritdoc />
    Task<bool> CreateKpiCalculationRequest(string symbol, string sourceDate, TokenModel? tokenModel);
}