using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.Models.KpiCalculation;
using StockTracker.Models.Persistence;

namespace StockTracker.MarketStack.Services.Contracts.Implementation;

public class StockKpiCalculator : IStockKpiCalculator
{
    private readonly IStockInfoRepository _stockInfoRepository;
    private readonly IAzureTableEntityResolver<StockInfoStorageTableKey> _tableEntityResolver;
    private readonly IStockKpiRepository _stockKpiRepository;
    private readonly ILogger<StockKpiCalculator> _logger;
    internal KpiFigure[] KpiCollection;

    public StockKpiCalculator(
        IStockInfoRepository stockInfoRepository,
        IAzureTableEntityResolver<StockInfoStorageTableKey> tableEntityResolver,
        IStockKpiRepository stockKpiRepository,
        ILogger<StockKpiCalculator> logger)
    {
        _stockInfoRepository = stockInfoRepository;
        _tableEntityResolver = tableEntityResolver;
        _stockKpiRepository = stockKpiRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<bool> CalculateKpis(string symbol, string currentDate, string previousDate)
    {
        _logger.LogInformation(
            $"Starting to perform KPI calculations for {symbol} with dates from {currentDate} to {previousDate}");
        var tableKeyCurrent = new StockInfoStorageTableKey {When = currentDate, Symbol = symbol};
        var currentInfo = await _stockInfoRepository.GetByIdAsync(tableKeyCurrent);

        var tableKeyPrevious = new StockInfoStorageTableKey {When = previousDate, Symbol = symbol};
        var previousInfo = await _stockInfoRepository.GetByIdAsync(tableKeyPrevious);

        KpiCollection =
        [
            new TrendToOpen(symbol),
            new TrendToClose(symbol),
            new TrendFromApertura(symbol),
            new Debilidad(symbol),
            new Fortaleza(symbol),
            new Interes(symbol)
        ];

        var calculatedKpiCollection = PerformKpiCalculation(currentDate, currentInfo, previousInfo);
        
        var result = await PersistCalculatedKpis(symbol, currentDate, previousDate, calculatedKpiCollection);

        return result;
    }

    /// <inheritdoc />
    public async Task<ReadOnlyDictionary<string, string>> GetPersistedKpiNamesBySymbol(string symbol)
    {
        KpiCollection =
        [
            new TrendToOpen(symbol),
            new TrendToClose(symbol),
            new TrendFromApertura(symbol),
            new Debilidad(symbol),
            new Fortaleza(symbol),
            new Interes(symbol)
        ];

        var persistedKpisBySymbol = await _stockKpiRepository.GetKpisBySymbolAsync(symbol);
        ReadOnlyDictionary<string, string> kpisCollection = BuildKpis(persistedKpisBySymbol);

        return kpisCollection;
    }
    
    /// <inheritdoc />
    public async ValueTask<IEnumerable<StockKpiModel>> GetPersistedKpiBySymbolByDateRange(string kpiSymbol,
        string from, string to)
    {
        var result = new List<StockKpiModel>();

        var fromPersisted = await _stockKpiRepository.GetKpiInfoByDateRange(kpiSymbol, from, to);
        result.AddRange(fromPersisted);

        return result;
    }
    
    /// <inheritdoc />
    public async ValueTask<IEnumerable<string>> GetKpiDatesBySymbol(string symbol)
    {
        var persistedKpisBySymbol = await _stockKpiRepository.GetKpisBySymbolAsync(symbol);
        if (!persistedKpisBySymbol.Any())
        {
            return Enumerable.Empty<string>();
        }

        // KPis by symbol are considered a whole package of KPIs defined in KpiCollection, ergo
        // extracting dates for first of the retrieved KPIs would be enough
        return await _stockKpiRepository.GetRowKeysByPartitionKeyAsync(persistedKpisBySymbol.First());
    }

    /// <inheritdoc />
    public async Task<bool> CleanupDeprecatedInfo(DateTime sourceDate)
    {
        bool removedFromStockInfo = await _stockInfoRepository.RemoveEntriesOlderThan(sourceDate);

        bool removedFromStockKpi = await _stockKpiRepository.RemoveEntriesOlderThan(sourceDate);

        return removedFromStockKpi && removedFromStockInfo;
    }

    private async Task<bool> PersistCalculatedKpis(string symbol, string currentDate, string previousDate,
        IEnumerable<StockKpiModel> calculatedKpiCollection)
    {
        var persistenceProcessExecution = new List<Tuple<string, bool>>();
        try
        {
            foreach (var stockKpiModel in calculatedKpiCollection)
            {
                var kpiPersisted = await _stockKpiRepository.CreateAsync(stockKpiModel);
                persistenceProcessExecution.Add(
                    new Tuple<string, bool>(stockKpiModel.SymbolKpiId, kpiPersisted));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"{nameof(PersistCalculatedKpis)}: Error calculating kpis for {symbol} with dates from {currentDate} to {previousDate}");
             
            await RollbackPersistedKpis(persistenceProcessExecution, currentDate);
        }
        
        return persistenceProcessExecution.TrueForAll(tuple => tuple.Item2);
    }

    private async Task RollbackPersistedKpis(List<Tuple<string, bool>> persistedKpis, string currentDate)
    {
        foreach (var tableKey in persistedKpis
                     .Select(persistedKpi => persistedKpi.Item1)
                     .Select(pk => new StockKpiStorageTableKey {When = currentDate, SymbolKpiId = pk}))
        {
            if (await _stockKpiRepository.ExistsAsync(tableKey))
            {
                await _stockKpiRepository.DeleteAsync(tableKey);
            }
        }
    }

    private IEnumerable<StockKpiModel> PerformKpiCalculation(string currentDate, StockInfoModel currentInfo, StockInfoModel previousInfo)
    {
        var kpicollection = new List<StockKpiModel>();

        //var kpicollection = new ConcurrentBag<StockKpiModel>();
        //Parallel.ForEach(KpiCollection, kpiFigure =>
        //{
        //    var item = new StockKpiModel();
        //    item.SymbolKpiId = kpiFigure.Id;
        //    item.When = currentDate;
        //    item.Result = kpiFigure switch
        //    {
        //        TrendToOpen => kpiFigure.Calculate(currentInfo.Open, previousInfo.Open),
        //        TrendToClose => kpiFigure.Calculate(currentInfo.Close, previousInfo.Close),
        //        TrendFromApertura => kpiFigure.Calculate(currentInfo.Close, previousInfo.Open),
        //        Interes => kpiFigure.Calculate(currentInfo.Open, previousInfo.Open),
        //        Fortaleza => kpiFigure.Calculate(currentInfo.High, currentInfo.Close),
        //        Debilidad => kpiFigure.Calculate(currentInfo.Close, currentInfo.Low),
        //        _ => throw new NotImplementedException("Empty collection of KPIs to calculate")
        //    };

        //    kpicollection.Add(item);
        //});
        
        foreach (var kpiFigure in KpiCollection)
        {
            var item = new StockKpiModel();
            item.SymbolKpiId = kpiFigure.ComposedId;
            item.When = currentDate;
            item.Result = kpiFigure switch
            {
                TrendToOpen => kpiFigure.Calculate(currentInfo.Open, previousInfo.Open),
                TrendToClose => kpiFigure.Calculate(currentInfo.Close, previousInfo.Close),
                TrendFromApertura => kpiFigure.Calculate(currentInfo.Close, previousInfo.Open),
                Interes => kpiFigure.Calculate(currentInfo.Open, previousInfo.Open),
                Fortaleza => kpiFigure.Calculate(currentInfo.High, currentInfo.Close),
                Debilidad => kpiFigure.Calculate(currentInfo.Close, currentInfo.Low),
                _ => throw new NotImplementedException("Empty collection of KPIs to calculate")
            };

            kpicollection.Add(item);
        }
        return kpicollection;

        //return kpicollection.ToArray();
    }

    private ReadOnlyDictionary<string, string> BuildKpis(IEnumerable<string> persistedKpisBySymbol)
    {
        var explainedKpis = new Dictionary<string, string>();
        foreach (var kpi in persistedKpisBySymbol)
        {
            var kpiInstance = KpiCollection.FirstOrDefault(figure =>
                figure.ComposedId.Equals(kpi, StringComparison.InvariantCultureIgnoreCase));

            var kpiExplanation = kpiInstance switch
            {
                TrendToOpen open => open.Explanation,
                TrendToClose close => close.Explanation,
                TrendFromApertura apertura => apertura.Explanation,
                Interes interes => interes.Explanation,
                Fortaleza fortaleza => fortaleza.Explanation,
                Debilidad debilidad => debilidad.Explanation,
                _ => throw new NotImplementedException("Not defined type of Kpi")
            };

            explainedKpis.Add(kpi, kpiExplanation);
        }

        return new ReadOnlyDictionary<string, string>(explainedKpis);
    }
}