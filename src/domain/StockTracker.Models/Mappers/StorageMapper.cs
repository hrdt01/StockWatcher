using StockTracker.CrossCutting.Utils;
using StockTracker.Models.Persistence;

namespace StockTracker.Models.Mappers;

public static class StorageMapper
{
    public static StockInfoStorageEntity ToStorageEntity(this StockInfo source, string extractorServiceName)
    {
        var rowKey = source.TradeEvents.FirstOrDefault()?.When.Date.ToRowKeyFormat();
        if (string.IsNullOrEmpty(rowKey))
            throw new ArgumentNullException($"No rowkey founded in conversion for {source.TickerSymbol}");

        return new StockInfoStorageEntity(source.TickerSymbol, rowKey)
        {
            ExtractorServiceName = extractorServiceName,
            Close = source.TradeEvents.FirstOrDefault()?.Close,
            High = source.TradeEvents.FirstOrDefault()?.High,
            Low = source.TradeEvents.FirstOrDefault()?.Low,
            Open = source.TradeEvents.FirstOrDefault()?.Open
        };
    }

    public static StockInfo ToDomainEntity(this StockInfoStorageEntity source)
    {
        var tradeInfo = new TradeEvent
        {
            Close = source.Close.GetValueOrDefault(),
            High = source.High.GetValueOrDefault(),
            Low = source.Low.GetValueOrDefault(),
            Open = source.Open.GetValueOrDefault(),
            When = DateTime.Parse(source.RowKey)
        };
        return new StockInfo(source.PartitionKey)
        {
            TradeEvents = [tradeInfo]
        };
    }

    public static StockInfoModel ToPersistenceEntity(this StockInfo source, string extractorServiceName)
    {
        var rowKey = source.TradeEvents.FirstOrDefault()?.When.Date.ToRowKeyFormat();
        if (string.IsNullOrEmpty(rowKey))
            throw new ArgumentNullException($"No rowkey founded in conversion for {source.TickerSymbol}");

        return new StockInfoModel
        {
            Symbol = source.TickerSymbol,
            When = rowKey,
            ExtractorServiceName = extractorServiceName,
            Close = Convert.ToDecimal(source.TradeEvents.FirstOrDefault()?.Close),
            High = Convert.ToDecimal(source.TradeEvents.FirstOrDefault()?.High),
            Low = Convert.ToDecimal(source.TradeEvents.FirstOrDefault()?.Low),
            Open = Convert.ToDecimal(source.TradeEvents.FirstOrDefault()?.Open)
        };
    }
}