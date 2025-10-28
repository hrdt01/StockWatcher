using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using StockTracker.Infrastructure.AzureTable.Definition;
using System.Linq.Expressions;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public abstract class AzureTableRepository<TEntity, TKey, TAzureTableEntity> : 
    IRepository<TEntity, TKey>, IAzureTableRepository<TEntity, TAzureTableEntity>
    where TEntity : class, new()
    where TAzureTableEntity : class, ITableEntity, new()
{
    protected readonly IAzureTableEntityResolver<TKey> _entityResolver;

    protected AzureTableClient _client;
    public abstract string TableName { get; }

    protected AzureTableRepository(
        IOptionsMonitor<AzureTableOptions> options, 
        IAzureTableEntityResolver<TKey> entityResolver)
    {
        _entityResolver = entityResolver ?? throw new ArgumentNullException(nameof(entityResolver));
        _client = new AzureTableClient(TableName, options);
    }

    protected abstract TEntity MapFromAzureTableEntity(TAzureTableEntity azureTableEntity);

    protected abstract TAzureTableEntity MapFromEntity(TEntity entity);
    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public virtual Task<bool> CreateAsync(TEntity entity)
    {
        var azureTableEntity = MapFromEntity(entity);
        return _client.CreateAsync(azureTableEntity);
    }

    public virtual Task<bool> UpdateAsync(TEntity entity)
    {
        var azureTableEntity = MapFromEntity(entity);
        return _client.UpdateAsync(azureTableEntity);
    }

    public virtual async Task<TEntity> GetByIdAsync(TKey key)
    {
        var partitionKey = _entityResolver.ResolvePartitionKey(key);
        var rowKey = _entityResolver.ResolveRowKey(key);

        var azureTableEntity = await _client.GetByIdAsync<TAzureTableEntity>(partitionKey, rowKey);

        var result = MapFromAzureTableEntity(azureTableEntity);
        return result;
    }

    public virtual Task<bool> ExistsAsync(TKey key)
    {
        var partitionKey = _entityResolver.ResolvePartitionKey(key);
        var rowKey = _entityResolver.ResolveRowKey(key);

        var result = _client.ExistsAsync(partitionKey, rowKey);
        return result;
    }

    public virtual Task<bool> DeleteAsync(TEntity entity)
    {
        var azureTableEntity = MapFromEntity(entity);
        return _client.DeleteAsync(azureTableEntity);
    }

    public virtual async Task<bool> DeleteAsync(IEnumerable<TEntity> entityCollection)
    {
        Dictionary<string, bool> operations = new Dictionary<string, bool>();
        foreach (var entity in entityCollection)
        {
            var azureTableEntity = MapFromEntity(entity);
            var operation = await _client.DeleteAsync(azureTableEntity);
            operations.Add($"{azureTableEntity.PartitionKey}_{azureTableEntity.RowKey}", operation);
        }

        var result = operations.All(pair => pair.Value);
        return result;
    }

    public virtual Task<bool> DeleteAsync(TKey key)
    {
        var azureTableEntity = new TAzureTableEntity
        {
            PartitionKey = _entityResolver.ResolvePartitionKey(key),
            RowKey = _entityResolver.ResolveRowKey(key),
            ETag = ETag.All
        };

        return _client.DeleteAsync(azureTableEntity);
    }

    public Task<IEnumerable<string>> GetAllPartitionsAsync(string searchPattern)
    {
        return _client.GetPartitionsAsync<TAzureTableEntity>(searchPattern);
    }

    public async Task<IEnumerable<string>> GetRowKeysByPartitionKeyAsync(string partitionKey)
    {
        var result = await _client.GetFromPartitionAsync<TAzureTableEntity>(partitionKey);
        return result.Select(entity => entity.RowKey);
    }

    public async Task<TEntity> GetFromPartitionRowAsync(string partitionKey, string rowKey)
    {
        var tableEntityResult = await _client.GetFromPartitionRowAsync<TAzureTableEntity>(partitionKey, rowKey);
        return MapFromAzureTableEntity(tableEntityResult);
    }

    public Task<IEnumerable<TEntity>> GetAllByQueryAsync(Expression<Func<TAzureTableEntity, bool>> queryToTable)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> GetPartitionsByPatternAsync(string searchPattern)
    {
        return _client.GetPartitionsByPatternAsync<TAzureTableEntity>(searchPattern);
    }

    public async Task<IEnumerable<TEntity>> GetByTimestampAsync(DateTime source)
    {
        var result = new List<TEntity>();
        var tableEntityResult = await _client.GetByTimestampAsync<TAzureTableEntity>(source);
        tableEntityResult.ToList().ForEach(item => result.Add(MapFromAzureTableEntity(item)));
        return result;
    }
}