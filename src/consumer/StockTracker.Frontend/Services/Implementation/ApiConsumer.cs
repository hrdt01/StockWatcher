using Polly;
using Polly.Registry;
using StockTracker.Frontend.Constants;
using StockTracker.Frontend.Services.Definition;
using System.Net.Http.Json;
using StockTracker.Frontend.Services.Models;
using System.Collections.ObjectModel;
using StockTracker.Frontend.Identity.Models;

namespace StockTracker.Frontend.Services.Implementation;

public class ApiConsumer : IApiConsumer
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ApiConsumer> _logger;
    private HttpClient _client;
    private ResiliencePipeline _resiliencePipeline;

    public ApiConsumer(
        IHttpClientFactory clientFactory,
        IServiceProvider serviceProvider,
        ILogger<ApiConsumer> logger)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException("ClientFactory is null.");
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException("ServiceProvider is null.");
        _logger = logger ?? throw new ArgumentNullException("Logger is null.");
        
        _client = _clientFactory.CreateClient(FrontendConstants.FrontendClientFactoryName);
        _resiliencePipeline = _serviceProvider
            .GetRequiredService<ResiliencePipelineProvider<string>>()
            .GetPipeline(FrontendConstants.ResiliencePipelineName);

    }

    /// <param name="tokenModel"></param>
    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetSymbols(TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetSymbolsApiEndpointRoute)}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetSymbols)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetSymbolsApiEndpointRoute}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetDatesBySymbol(string symbol, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetDatesBySymbolApiEndpointRoute)}?symbol={symbol}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetDatesBySymbol)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetDatesBySymbolApiEndpointRoute} " +
                $"for symbol {symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

        return result;
    }

    /// <inheritdoc />
    public async Task<StockInfoResponse> GetInfoBySymbolDateRange(string symbol, string from, string to, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetInfoBySymbolDateRangeApiEndpointRoute)}" +
            $"?symbol={symbol}&from={from}&to={to}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetInfoBySymbolDateRange)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetInfoBySymbolDateRangeApiEndpointRoute} " +
                $"for symbol {symbol} and date range:{from} --> {to}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<StockInfoResponse>();

        return result;
    }

    /// <inheritdoc />
    public async Task<StockKpiResponse> GetKpisBySymbolDateRange(string symbolKpi, string from, string to, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetKpisBySymbolDateRangeApiEndpointRoute)}" +
            $"?symbolKpi={symbolKpi}&from={from}&to={to}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetKpisBySymbolDateRange)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetKpisBySymbolDateRangeApiEndpointRoute} " +
                $"for kpi {symbolKpi} and date range:{from} --> {to}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<StockKpiResponse>();

        return result;
    }

    /// <inheritdoc />
    public async Task<ReadOnlyCollection<TrackedCompany>> GetTrackedCompanies(bool enabled, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetTrackedCompaniesApiEndpointRoute)}?enabled={enabled}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetTrackedCompanies)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetTrackedCompaniesApiEndpointRoute}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<TrackedCompany>>();

        return new ReadOnlyCollection<TrackedCompany>(result.ToList());
    }

    /// <inheritdoc />
    public async Task<ReadOnlyDictionary<string, string>> GetKpisBySymbol(string symbol, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetKpisBySymbolApiEndpointRoute)}?symbol={symbol}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetKpisBySymbol)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetKpisBySymbolApiEndpointRoute} " +
                $"for symbol {symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        return new ReadOnlyDictionary<string, string>(result);
    }

    /// <inheritdoc />
    public async Task<bool> SaveTrackedCompany(TrackedCompany source, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.PostSaveTrackedCompanyApiEndpointRoute)}";
        if (string.IsNullOrWhiteSpace(source.PseudoRowKey))
        {
            source.PseudoRowKey = Guid.NewGuid().ToString("N");
        }

        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.PostAsJsonAsync(finalUrl, source, ct);
            },
            _client);

        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(SaveTrackedCompany)}: " +
                $"Error persisting information for endpoint {FrontendConstants.PostSaveTrackedCompanyApiEndpointRoute} " +
                $"for tracked company {source.Symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetKpiDatesBySymbol(string symbol, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.GetKpiDatesBySymbolApiEndpointRoute)}?symbol={symbol}";
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.GetAsync(finalUrl, ct);
            },
            _client);
        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(GetKpiDatesBySymbol)}: " +
                $"Error retrieving information for endpoint {FrontendConstants.GetKpiDatesBySymbolApiEndpointRoute} " +
                $"for symbol {symbol}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        var result = await apiResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> CreateKpiCalculationRequest(string symbol, string sourceDate, TokenModel? tokenModel)
    {
        var finalUrl =
            $"{BuildEndpointUrlRequest(FrontendConstants.CreateKpiCalculationApiEndpointRoute)}";
        
        var apiResponse = await _resiliencePipeline.ExecuteAsync(
            async (httpClient, ct) =>
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    tokenModel != null
                        ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenModel.AccessToken)
                        : null;
                return await httpClient.PostAsJsonAsync(finalUrl,
                    new KpiProcessRequest{ ProcessDate = sourceDate, Symbol = symbol }
                    , ct);
            },
            _client);

        if (!apiResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(CreateKpiCalculationRequest)}: " +
                $"Error persisting information for endpoint {FrontendConstants.CreateKpiCalculationApiEndpointRoute} " +
                $"for symbol {symbol} and date {sourceDate}");

            throw new Exception($"{apiResponse.StatusCode}: {apiResponse.ReasonPhrase}");
        }

        return true;
    }

    private string BuildEndpointUrlRequest(string endpointRoute)
    {
        return $"{endpointRoute}";
    }
}