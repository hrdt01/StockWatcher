using Microsoft.Extensions.DependencyInjection;
using Polly.Registry;
using StockTracker.MarketStack.Services.Contracts.Definition;
using StockTracker.MarketStack.Services.Models;
using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Models;
using StockTracker.Models.Mappers;
using StockTracker.CrossCutting.Constants;
using StockTracker.Models.ApiModels;
using StockTracker.Models.Persistence;

namespace StockTracker.MarketStack.Services.Contracts.Implementation;

public class StockTracker : IStockTracker
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStockInfoRepository _stockInfoRepository;
    private readonly IAzureTableEntityResolver<StockInfoStorageTableKey> _tableEntityResolver;
    private readonly IConfiguration _configuration;
    private readonly ITrackedCompanyRepository _trackedCompanyRepository;
    private readonly ILogger<StockTracker> _logger;
    private readonly string? _accessKey;

    public StockTracker(
        IHttpClientFactory clientFactory, 
        IServiceProvider serviceProvider,
        IStockInfoRepository stockInfoRepository,
        IAzureTableEntityResolver<StockInfoStorageTableKey> tableEntityResolver,
        IConfiguration configuration,
        ITrackedCompanyRepository trackedCompanyRepository,
        ILogger<StockTracker> logger)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException("ClientFactory is null.");
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException("ServiceProvider is null.");
        _stockInfoRepository = stockInfoRepository ?? throw new ArgumentNullException("Repository is null.");
        _tableEntityResolver = tableEntityResolver ?? throw new ArgumentNullException("EntityResolver is null.");
        _configuration = configuration ?? throw new ArgumentNullException("Configuration is null.");
        _trackedCompanyRepository = trackedCompanyRepository ?? throw new ArgumentNullException("Repository is null.");
        _logger = logger ?? throw new ArgumentNullException("Logger is null.");
        _accessKey = _configuration.GetValue<string>(ApiConstants.AccessKeySettingName);
    }

    /// <inheritdoc />
    public async ValueTask<EndOfDayResponse?> GetEndOfDayBySymbol(string tickerSymbol, DateTime targetDay)
    {
        var client = _clientFactory.CreateClient(ApiConstants.ClientFactoryName);

        var resiliencePipeline = _serviceProvider
            .GetRequiredService<ResiliencePipelineProvider<string>>()
            .GetPipeline(ApiConstants.ResiliencePipelineName);

        var apiResponse = await resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                var finalUrl =
                    $"{BuildEndOfDayEndpointRequest(_accessKey, targetDay)}&{ApiConstants.RequiredSymbolsQueryParam}={tickerSymbol}";
                return await httpClient.GetAsync(finalUrl, ct);
            }
            ,
            client);
        
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetEndOfDayBySymbol)}: Error retrieving information for symbol {tickerSymbol} and date {targetDay}");
            
            throw new BadRequestException($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }
        var result = await apiResponse.Content.ReadFromJsonAsync<EndOfDayResponse>();

        return result;
    }

    /// <inheritdoc />
    public async ValueTask<bool> ConsolidateEndOfDayInfo(EndOfDayResponse sourceData, string tickerSymbol, DateTime targetDay)
    {
        bool result = false;
        if (!sourceData.data.Any())
        {
            _logger.LogWarning($"No data found for symbol:{tickerSymbol} at: {targetDay.Date}");
            return result;
        }

        try
        {
            // Perform translation to domain model
            var stockInfoInstance = new StockInfo(tickerSymbol);

            foreach (var endOfDaySymbol in sourceData.data)
            {
                stockInfoInstance.TradeEvents.Add(
                    new TradeEvent()
                    {
                        Close = endOfDaySymbol.close,
                        High = endOfDaySymbol.high,
                        Low = endOfDaySymbol.low,
                        Open = endOfDaySymbol.open,
                        When = targetDay
                    }
                );
            }

            // Perform persistence in Table storage
            var persistenceEntity = stockInfoInstance.ToPersistenceEntity(ApiConstants.ExtractorServiceName);
            var exists = await _stockInfoRepository.ExistsAsync(persistenceEntity);

            if (!exists)
                result = await _stockInfoRepository.CreateAsync(persistenceEntity);
            else
                result = await _stockInfoRepository.UpdateAsync(persistenceEntity);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(ConsolidateEndOfDayInfo)}: Error consolidating information in persistence layer for symbol {tickerSymbol} and date {targetDay}");
            throw;
        }
        return result;
    }

    /// <inheritdoc />
    public string BuildEndOfDayEndpointRequest(string accessKey, DateTime targetDay)
    {
        return
            $"{ApiConstants.EndOfDayEndpointUrl}/{targetDay.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}?{ApiConstants.RequiredAccessKeyQueryParam}={accessKey}";
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<string>> GetAllSymbolsAsync(string searchPattern = "BMEX")
    {
        var result = Enumerable.Empty<string>();
        try
        {
            result = await _stockInfoRepository.GetAllSymbolsAsync(searchPattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(GetAllSymbolsAsync)}: Error retrieving all persisted symbols.");
            throw;
        }

        return result;
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<string>> GetDatesBySymbolAsync(string symbol)
    {
        var result = Enumerable.Empty<string>();
        try
        {
            result = await _stockInfoRepository.GetDatesBySymbolAsync(symbol);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(GetDatesBySymbolAsync)}: Error retrieving all persisted dates for symbol: {symbol}.");
            throw;
        }

        return result;
    }

    /// <inheritdoc />
    public async ValueTask<StockInfoResponse> GetStockInfoByDateRange(string symbol, string from, string to)
    {
        var result = new StockInfoResponse(symbol);

        try
        {
            var persistedStockInfo= 
                await _stockInfoRepository.GetStockInfoByDateRange(symbol, from, to);
            foreach (var stockInfoModel in persistedStockInfo)
            {
                result.TradeEvents.Add(new TradeEvent()
                {
                    High = Convert.ToDouble(stockInfoModel.High),
                    Close = Convert.ToDouble(stockInfoModel.Close),
                    Low = Convert.ToDouble(stockInfoModel.Low),
                    Open = Convert.ToDouble(stockInfoModel.Open),
                    When = DateTime.Parse(stockInfoModel.When)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(GetStockInfoByDateRange)}: Error retrieving persisted info for symbol: {symbol} between {from} and {to}.");
            throw;
        }

        return result;
    }
    
    /// <inheritdoc />
    public async ValueTask<IEnumerable<TrackedCompanyModel>> GetTrackedCompanies(bool enabled = false)
    {
        try
        {
            return  await _trackedCompanyRepository.GetTrackedCompaniesAsync(enabled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(GetTrackedCompanies)}: Error retrieving tracked companies with enabled value: {enabled}.");
            return Enumerable.Empty<TrackedCompanyModel>();
        }
    }
    
    /// <inheritdoc />
    public async ValueTask<bool> ExecuteTrackedCompaniesInitialMigration()
    {
        var trackedCompaniesInitialList =
            new List<TrackedCompanyModel>()
            {
                new()
                {
                    Enabled = true, Name = "Obrascón Huarte Lain, S.A.", Url = "https://www.bolsamania.com/accion/OHLA",
                    PseudoRowKey = Guid.NewGuid().ToString("N"), Symbol = "OHLA.BMEX"
                },
                new()
                {
                    Enabled = true, Name = "Grenergy Renovables",
                    Url = "https://www.bolsamania.com/accion/GRENERGY-RENOVAB",
                    PseudoRowKey = Guid.NewGuid().ToString("N"), Symbol = "GRE.BMEX"
                },
                new()
                {
                    Enabled = true, Name = "IAG", Url = "https://www.bolsamania.com/accion/INTL-CONS-AIR",
                    PseudoRowKey = Guid.NewGuid().ToString("N"), Symbol = "IAG.BMEX"
                }
            };
        try
        {
            foreach (var trackedCompanyModel in trackedCompaniesInitialList)
            {
                bool trackedCompanyAlreadyExist = await CheckExistingCompany(trackedCompanyModel.Symbol);
               
                if (trackedCompanyAlreadyExist)
                    continue;
                var tableKey =
                    new TrackedCompanyStorageTableKey
                    {
                        Symbol = trackedCompanyModel.Symbol,
                        PseudoRowKey = trackedCompanyModel.PseudoRowKey
                    };
                var exist = await _trackedCompanyRepository.ExistsAsync(tableKey);
                if (!exist)
                    _ = _trackedCompanyRepository.CreateAsync(trackedCompanyModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(ExecuteTrackedCompaniesInitialMigration)}: Error inserting initial tracked companies.");
            return false;
        }

        return true;
    }
    
    /// <inheritdoc />
    public async Task<bool> SaveTrackedCompany(TrackedCompanyModel source)
    {
        var result = false;
        try
        {
            var entityTableKey = new TrackedCompanyStorageTableKey
            {
                Symbol = source.Symbol, PseudoRowKey = source.PseudoRowKey
            };

            var existing = await _trackedCompanyRepository.ExistsAsync(entityTableKey);
            result = !existing
                ? await _trackedCompanyRepository.CreateAsync(source)
                : await _trackedCompanyRepository.UpdateAsync(source);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{nameof(SaveTrackedCompany)}: Error persisting tracked company.");
            return false;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> CheckExistingCompany(string symbol)
    {
        var persistedPartitions = await _trackedCompanyRepository.GetAllPartitionsAsync(string.Empty);
        return persistedPartitions.Contains(symbol);
    }
}