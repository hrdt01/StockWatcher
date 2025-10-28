using Polly;
using Polly.Registry;
using StockTracker.CrossCutting.Constants;
using System.Collections.ObjectModel;
using StockTracker.Models.FrontendModels;

namespace StockTracker.Identity.Api.Areas.Tracker.Services;

public class TrackerService : ITrackerService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TrackerService> _logger;
    private string? _apiKey;
    private HttpClient _client;
    private ResiliencePipeline _resiliencePipeline;

    public TrackerService(
        IHttpClientFactory clientFactory,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<TrackerService> logger)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException("ClientFactory is null.");
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException("ServiceProvider is null.");
        _configuration = configuration ?? throw new ArgumentNullException("Configuration is null.");
        _logger = logger ?? throw new ArgumentNullException("Logger is null.");

        _apiKey = _configuration.GetValue<string>(ApiConstants.ApiKeySettingName);
        _client = _clientFactory.CreateClient(ApiConstants.FrontendClientFactoryName);
        _resiliencePipeline = _serviceProvider
            .GetRequiredService<ResiliencePipelineProvider<string>>()
            .GetPipeline(ApiConstants.FunctionResiliencePipelineName);

    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetSymbols()
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetSymbolsApiEndpointRoute, _apiKey)}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) => await httpClient.GetAsync(finalUrl, ct),
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetSymbols)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetSymbolsApiEndpointRoute} and apiKey {_apiKey}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

        return result;
    }
    
    /// <inheritdoc />
    public async Task<ReadOnlyCollection<TrackedCompany>> GetTrackedCompanies(bool enabled)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetTrackedCompaniesApiEndpointRoute, _apiKey)}&enabled={enabled}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetTrackedCompanies)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetTrackedCompaniesApiEndpointRoute} " +
                $"and apiKey {_apiKey}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<TrackedCompany>>();

        return new ReadOnlyCollection<TrackedCompany>(result.ToList());
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetDatesBySymbol(string symbol)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetDatesBySymbolApiEndpointRoute, _apiKey)}&symbol={symbol}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) => await httpClient.GetAsync(finalUrl, ct),
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetDatesBySymbol)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetDatesBySymbolApiEndpointRoute} " +
                $"and apiKey {_apiKey} for symbol {symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

        return result;
    }

    /// <inheritdoc />
    public async Task<StockInfoResponse> GetInfoBySymbolDateRange(string symbol, string from, string to)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetInfoBySymbolDataRangeApiEndpointRoute, _apiKey)}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                return await httpClient.PostAsJsonAsync(finalUrl,
                    new
                    {
                        symbol = symbol,
                        from = from,
                        to = to
                    }, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetInfoBySymbolDateRange)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetInfoBySymbolDataRangeApiEndpointRoute} " +
                $"and apiKey {_apiKey} for symbol {symbol} and date range:{from} --> {to}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<StockInfoResponse>();

        return result;
    }

    /// <inheritdoc />
    public async Task<StockKpiResponse> GetKpisBySymbolDateRange(string symbolKpi, string from, string to)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetKpisBySymbolDateRangeApiEndpointRoute, _apiKey)}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                return await httpClient.PostAsJsonAsync(finalUrl,
                    new
                    {
                        symbolKpi = symbolKpi,
                        from = from,
                        to = to
                    }, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetKpisBySymbolDateRange)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetKpisBySymbolDateRangeApiEndpointRoute} " +
                $"and apiKey {_apiKey} for kpi {symbolKpi} and date range:{from} --> {to}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<StockKpiResponse>();

        return result;
    }
    
    /// <inheritdoc />
    public async Task<ReadOnlyDictionary<string, string>> GetKpisBySymbol(string symbol)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetKpisBySymbolApiEndpointRoute, _apiKey)}&symbol={symbol}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) => await httpClient.GetAsync(finalUrl, ct),
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetKpisBySymbol)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetKpisBySymbolApiEndpointRoute} " +
                $"and apiKey {_apiKey} for symbol {symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        return new ReadOnlyDictionary<string, string>(result);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetKpiDatesBySymbol(string symbol)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.GetKpiDatesBySymbolApiEndpointRoute, _apiKey)}&symbol={symbol}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) => await httpClient.GetAsync(finalUrl, ct),
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetKpiDatesBySymbol)}: " +
                $"Error retrieving information for endpoint {ApiConstants.GetKpiDatesBySymbolApiEndpointRoute} " +
                $"and apiKey {_apiKey} for symbol {symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

        return result;
    }
    
    /// <inheritdoc />
    public async Task<bool> SaveTrackedCompany(TrackedCompany source)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.PostSaveTrackedCompanyApiEndpointRoute, _apiKey)}";
        if (string.IsNullOrWhiteSpace(source.PseudoRowKey))
        {
            source.PseudoRowKey = Guid.NewGuid().ToString("N");
        }

        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                return await httpClient.PostAsJsonAsync(finalUrl,
                    new
                    {
                        enabled = source.Enabled,
                        name = source.Name,
                        symbol = source.Symbol,
                        url = source.Url,
                        pseudoRowKey = source.PseudoRowKey
                    }, ct);
            },
            _client);

        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(SaveTrackedCompany)}: " +
                $"Error persisting information for endpoint {ApiConstants.PostSaveTrackedCompanyApiEndpointRoute} " +
                $"and apiKey {_apiKey} for tracked company {source.Symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CreateKpiCalculationRequest(KpiProcessRequest source)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(ApiConstants.CreateKpiCalculationApiEndpointRoute, _apiKey)}";

        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                return await httpClient.PostAsJsonAsync(finalUrl,
                    new
                    {
                        symbol = source.Symbol,
                        processDate = source.ProcessDate
                    }, ct);
            },
            _client);

        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(CreateKpiCalculationRequest)}: " +
                $"Error persisting information for endpoint {ApiConstants.CreateKpiCalculationApiEndpointRoute} " +
                $"and apiKey {_apiKey} for symbol {source.Symbol} and date {source.ProcessDate}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        return true;
    }

    private string BuildEndpointUrlRequest(string endpointRoute, string? apiKey)
    {
        return $"api/{endpointRoute}?code={apiKey}";
    }
}