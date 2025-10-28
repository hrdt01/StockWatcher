using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Net;
using StockTracker.CrossCutting.ExceptionHandling.RepositoryExceptions;

namespace StockTracker.Infrastructure.AzureTable.Implementation;

public class AzureTableClient
{
    private readonly IOptionsMonitor<AzureTableOptions> _options;

    public TableClient TableClient => _tableLazy.Value;
    private readonly Lazy<TableClient> _tableLazy;
    public TableServiceClient Client => _tableClientLazy.Value;
    private readonly Lazy<TableServiceClient> _tableClientLazy;

    public const string PartitionKey = "PartitionKey";
    public const string RowKey = "RowKey";

    public AzureTableClient(string tableName, IOptionsMonitor<AzureTableOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _tableLazy = new Lazy<TableClient>(() => CreateCloudTableClient(tableName));
        _tableClientLazy = new Lazy<TableServiceClient>(CreateCloudTableServiceClient);
    }
    
    private TableClient CreateCloudTableClient(string tableName)
    {
        var tableClient = Client.GetTableClient(tableName);
        
        var cloudTableReference= tableClient.CreateIfNotExists();
        
        return tableClient;
    }
    private TableServiceClient CreateCloudTableServiceClient()
    {
        return new TableServiceClient(_options.CurrentValue.ConnectionString);
    }

    public virtual async Task<bool> CreateAsync(ITableEntity entity)
    {
        try
        {
            var insertOperation = new[] {new TableTransactionAction(TableTransactionActionType.Add, entity, ETag.All) };
            await TableClient.SubmitTransactionAsync(insertOperation);
            return true;
        }
        catch (RequestFailedException exception)
        {
            throw HandleException(exception, TableTransactionActionType.Add, entity);
        }
    }
    public virtual async Task<bool> UpdateAsync(ITableEntity entity)
    {
        try
        {
            var updateOperation = new[] { new TableTransactionAction(TableTransactionActionType.UpdateReplace, entity, ETag.All) };
            await TableClient.SubmitTransactionAsync(updateOperation);
            return true;
        }
        catch (RequestFailedException exception)
        {
            throw HandleException(exception, TableTransactionActionType.UpdateReplace, entity);
        }
    }
    public virtual async Task<bool> DeleteAsync(ITableEntity entity)
    {
        try
        {
            var deleteOperation = new[] { new TableTransactionAction(TableTransactionActionType.Delete, entity, ETag.All) }; 
            await TableClient.SubmitTransactionAsync(deleteOperation);
            return true;
        }
        catch (RequestFailedException exception)
        {
            throw HandleException(exception, TableTransactionActionType.Delete, entity);
        }
    }
    public virtual async Task<bool> CreateAsync(IEnumerable<ITableEntity> entities)
    {
        try
        {
            var batch = new List<TableTransactionAction>();
            batch.AddRange(entities.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));

            await TableClient.SubmitTransactionAsync(batch);
            return true;
        }
        catch (RequestFailedException exception)
        {
            throw HandleException(exception, TableTransactionActionType.Add, entities.FirstOrDefault());
        }
    }
    public virtual async Task<bool> UpdateAsync(IEnumerable<ITableEntity> entities)
    {
        try
        {
            var batch = new List<TableTransactionAction>();
            batch.AddRange(entities.Select(e => new TableTransactionAction(TableTransactionActionType.UpdateReplace, e)));

            await TableClient.SubmitTransactionAsync(batch);
            return true;
        }
        catch (RequestFailedException exception)
        {
            throw HandleException(exception, TableTransactionActionType.UpdateReplace, entities.FirstOrDefault());
        }
    }
    public virtual async Task<bool> DeleteAsync(IEnumerable<ITableEntity> entities)
    {
        try
        {
            var batch = new List<TableTransactionAction>();
            batch.AddRange(entities.Select(e => new TableTransactionAction(TableTransactionActionType.Delete, e)));

            await TableClient.SubmitTransactionAsync(batch);
            return true;
        }
        catch (RequestFailedException exception)
        {
            throw HandleException(exception, TableTransactionActionType.Delete, entities.FirstOrDefault());
        }
    }
    public virtual async Task<TAzureTableEntity?> GetByIdAsync<TAzureTableEntity>(string partitionKey, string rowKey)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        //var query = await TableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey);
        //var result = query.Value;

        //if (result == null)
        //{
        //    throw new EntityNotFoundException($"({partitionKey}, {rowKey})");
        //}

        //return result as TAzureTableEntity;

        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => entity.PartitionKey == partitionKey && entity.RowKey == rowKey
        );

        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        TAzureTableEntity? result = null;

        await foreach (Page<TAzureTableEntity> page in queryOperation.AsPages())
        {
            result = page.Values.FirstOrDefault();
            break;
        }

        if (result == null)
        {
            throw new EntityNotFoundException($"({partitionKey}, {rowKey})");
        }

        return result;
    }
    public virtual async Task<bool> ExistsAsync(string partitionKey, string rowKey)
    {
        var query = await TableClient.GetEntityIfExistsAsync<TableEntity>(partitionKey, rowKey);
        
        return query.HasValue;
    }
    public virtual async Task<IList<TAzureTableEntity>> GetFromPartitionAsync<TAzureTableEntity>(string partitionKey)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => entity.PartitionKey == partitionKey);

        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        IList<TAzureTableEntity> result = new List<TAzureTableEntity>();

        string continuationToken = null;
        while (true)
        {
            var page = await GetPageAsync(queryOperation, continuationToken);
            foreach (TAzureTableEntity tableEntity in page.Values)
            {
                result.Add(tableEntity);
            }
            if (string.IsNullOrEmpty(page.ContinuationToken))
            {
                break;
            }
            continuationToken = page.ContinuationToken;
        }

        return result;
    }
    public virtual async Task<IList<TAzureTableEntity>> GetFromRowAsync<TAzureTableEntity>(string rowKey)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => entity.RowKey == rowKey);

        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        IList<TAzureTableEntity> result = new List<TAzureTableEntity>();

        string continuationToken = null;
        while (true)
        {
            var page = await GetPageAsync(queryOperation, continuationToken);
            foreach (TAzureTableEntity tableEntity in page.Values)
            {
                result.Add(tableEntity);
            }
            if (string.IsNullOrEmpty(page.ContinuationToken))
            {
                break;
            }
            continuationToken = page.ContinuationToken;
        }

        return result;
    }

    public virtual async Task<TAzureTableEntity?>  GetFromPartitionRowAsync<TAzureTableEntity>(string partitionKey, string rowKey)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => entity.PartitionKey == partitionKey && entity.RowKey == rowKey);

        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        TAzureTableEntity? result = null;

        await foreach (Page<TAzureTableEntity> page in queryOperation.AsPages())
        {
            result = page.Values.FirstOrDefault();
            break;
        }

        if (result == null)
        {
            throw new EntityNotFoundException($"({partitionKey}, {rowKey})");
        }

        return result;
    }
    public virtual async Task<IEnumerable<string>> GetPartitionsAsync<TAzureTableEntity>(string partitionKeyPattern)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => string.Compare(entity.PartitionKey, string.Empty) > 0);

        // string.Compare(entity.PartitionKey, "") > 0
        //&& string.Compare($"0000.{partitionKeyPattern}", entity.PartitionKey) < 0

        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        IList<string> result = new List<string>();

        string continuationToken = null;
        while (true)
        {
            var page = await GetPageAsync(queryOperation, continuationToken);
            foreach (TAzureTableEntity tableEntity in page.Values)
            {
                if (!result.Any(item =>
                        item.Equals(tableEntity.PartitionKey, StringComparison.InvariantCultureIgnoreCase)))
                    result.Add(tableEntity.PartitionKey);
            }
            if (string.IsNullOrEmpty(page.ContinuationToken))
            {
                break;
            }
            continuationToken = page.ContinuationToken;
        }

        return result;
    }

    public virtual async Task<IEnumerable<string>> GetPartitionsByPatternAsync<TAzureTableEntity>(string partitionKeyPattern)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => string.Compare(entity.PartitionKey, partitionKeyPattern) > 0);
        
        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        IList<string> result = new List<string>();

        string continuationToken = null;
        while (true)
        {
            var page = await GetPageAsync(queryOperation, continuationToken);
            foreach (TAzureTableEntity tableEntity in page.Values)
            {
                if (tableEntity.PartitionKey.StartsWith(partitionKeyPattern, StringComparison.InvariantCultureIgnoreCase) &&
                    !result.Any(item =>
                        item.Equals(tableEntity.PartitionKey, StringComparison.InvariantCultureIgnoreCase)))
                    result.Add(tableEntity.PartitionKey);
            }
            if (string.IsNullOrEmpty(page.ContinuationToken))
            {
                break;
            }
            continuationToken = page.ContinuationToken;
        }

        return result;
    }

    public virtual async Task<Page<TAzureTableEntity>> GetPageAsync<TAzureTableEntity>(AsyncPageable<TAzureTableEntity> pageable, string continuationToken)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        await using (IAsyncEnumerator<Page<TAzureTableEntity>> enumerator = pageable.AsPages(continuationToken).GetAsyncEnumerator())
        {
            await enumerator.MoveNextAsync();
            return enumerator.Current;
        }
    }
    
    public virtual async Task<IList<TAzureTableEntity>> GetByTimestampAsync<TAzureTableEntity>(DateTime source)
        where TAzureTableEntity : class, ITableEntity, new()
    {
        var query = TableClient.CreateQueryFilter<TAzureTableEntity>(
            entity => entity.Timestamp<= source);

        var queryOperation = TableClient.QueryAsync<TAzureTableEntity>(filter: query, maxPerPage: 1000);
        IList<TAzureTableEntity> result = new List<TAzureTableEntity>();

        string continuationToken = null;
        while (true)
        {
            var page = await GetPageAsync(queryOperation, continuationToken);
            foreach (TAzureTableEntity tableEntity in page.Values)
            {
                result.Add(tableEntity);
            }
            if (string.IsNullOrEmpty(page.ContinuationToken))
            {
                break;
            }
            continuationToken = page.ContinuationToken;
        }

        return result;
    }

    public Exception HandleException(RequestFailedException exception, TableTransactionActionType operationType, ITableEntity tableEntity)
    {
        return HandleException(exception, operationType, tableEntity.PartitionKey, tableEntity.RowKey);
    }

    public Exception HandleException(RequestFailedException exception, TableTransactionActionType operationType, string partitionKey, string rowKey)
    {
        var httpStatusCode = (HttpStatusCode)exception.Status;
        var errorCode = exception.ErrorCode;

        if (operationType == TableTransactionActionType.Add)
        {
            if (httpStatusCode == HttpStatusCode.Conflict && errorCode == "EntityAlreadyExists")
            {
                return new EntityAlreadyExistsException($"{operationType} with key: ({partitionKey}, {rowKey})");
            }
        }
        else if (operationType == TableTransactionActionType.Delete || 
                 operationType == TableTransactionActionType.UpdateMerge || 
                 operationType == TableTransactionActionType.UpdateReplace ||
                 operationType == TableTransactionActionType.UpsertMerge ||
                 operationType == TableTransactionActionType.UpsertReplace)
        {
            if (httpStatusCode == HttpStatusCode.NotFound && errorCode == "ResourceNotFound")
            {
                return new EntityNotFoundException($"{operationType} with key: ({partitionKey}, {rowKey})");
            }
        }

        return exception;
    }
}