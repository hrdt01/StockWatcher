namespace StockTracker.Models.Persistence;

/// <summary>
/// Contains the custom properties that will map with Azure table keys
/// </summary>
public class StockInfoStorageTableKey
{
    /// <summary>
    /// A string containing the partition key for the entity.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// A string containing the row key for the entity.
    /// </summary>
    public string When { get; set; }
}