using StockTracker.Models.ApiModels;
using StockTracker.Models.Persistence;

namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface ITrackedCompanyRepository : IRepository<TrackedCompanyModel, TrackedCompanyStorageTableKey>
{
    Task<IEnumerable<TrackedCompanyModel>> GetTrackedCompaniesAsync(bool enabled = false);
}