using System.Collections.ObjectModel;
using StockTracker.Models.FrontendModels;

namespace StockTracker.Identity.Api.Areas.Tracker.Services;

public interface ITrackerService
{
    /// <inheritdoc />
    Task<IEnumerable<string>> GetSymbols();

    /// <inheritdoc />
    Task<ReadOnlyCollection<TrackedCompany>> GetTrackedCompanies(bool enabled);

    /// <inheritdoc />
    Task<IEnumerable<string>> GetDatesBySymbol(string symbol);

    /// <inheritdoc />
    Task<StockInfoResponse> GetInfoBySymbolDateRange(string symbol, string from, string to);

    /// <inheritdoc />
    Task<StockKpiResponse> GetKpisBySymbolDateRange(string symbolKpi, string from, string to);

    /// <inheritdoc />
    Task<ReadOnlyDictionary<string, string>> GetKpisBySymbol(string symbol);

    /// <inheritdoc />
    Task<IEnumerable<string>> GetKpiDatesBySymbol(string symbol);

    /// <inheritdoc />
    Task<bool> SaveTrackedCompany(TrackedCompany source);

    /// <inheritdoc />
    Task<bool> CreateKpiCalculationRequest(KpiProcessRequest source);
}