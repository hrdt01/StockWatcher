using Microsoft.Extensions.Options;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public abstract class AzureQueueRepository<TEntity>: IAzureQueueRepository<TEntity>
    where TEntity : class, IRequestContract, new()
{

    protected AzureQueueClient _client;
    public abstract string QueueName { get; }

    protected AzureQueueRepository(IOptionsMonitor<AzureQueueOptions> options)
    {
        _client = new AzureQueueClient(QueueName, options);
    }

    public virtual async Task<bool> InsertMessageAsync(TEntity entity)
    {
        return await _client.InsertMessageAsync<TEntity>(entity);
    }
}