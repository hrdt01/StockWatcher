namespace StockTracker.Infrastructure.AzureTable.Definition;

public interface IAzureTableRepository<TEntity, TAzureTableEntity>
{
    Task<IEnumerable<TEntity>> GetAllByQueryAsync(System.Linq.Expressions.Expression<Func<TAzureTableEntity, bool>> queryToTable);
}