namespace StockTracker.Infrastructure;

public interface IRepository<TEntity, in TKey>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(TKey key);
    Task<bool> CreateAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> ExistsAsync(TKey key);
    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(IEnumerable<TEntity> entityCollection);
    Task<bool> DeleteAsync(TKey key);
    Task<IEnumerable<string>> GetAllPartitionsAsync(string searchPattern);
    Task<IEnumerable<string>> GetRowKeysByPartitionKeyAsync(string partitionKey);
    Task<TEntity> GetFromPartitionRowAsync(string partitionKey, string rowKey);
    Task<IEnumerable<TEntity>> GetByTimestampAsync(DateTime source);
}