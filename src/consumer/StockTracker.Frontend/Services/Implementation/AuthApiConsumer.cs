using Polly;
using Polly.Registry;
using StockTracker.Frontend.Constants;
using StockTracker.Frontend.Identity.Models;
using StockTracker.Frontend.Services.Definition;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

namespace StockTracker.Frontend.Services.Implementation;

public class AuthApiConsumer : IAuthApiConsumer
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuthApiConsumer> _logger;
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline _resiliencePipeline;

    public AuthApiConsumer(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<AuthApiConsumer> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException("ClientFactory is null.");
        _configuration = configuration ?? throw new ArgumentNullException("Configuration is null.");
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException("ServiceProvider is null.");
        _logger = logger ?? throw new ArgumentNullException("Logger is null.");

        _httpClient = _httpClientFactory.CreateClient(FrontendConstants.AuthClientFactoryName);
        _resiliencePipeline = _serviceProvider
            .GetRequiredService<ResiliencePipelineProvider<string>>()
            .GetPipeline(FrontendConstants.ResiliencePipelineName);

    }

    /// <inheritdoc />
    public async Task<TokenModel?> LoginAsync(string email, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, FrontendConstants.LoginApiEndpointRoute)
        {
            Content = JsonContent.Create(new Dictionary<string, string>
            {
                { "username", email },
                { "password", password }
            })
        };
        var response = await _resiliencePipeline.ExecuteAsync(
            async ct => await _httpClient.SendAsync(request, ct),
            CancellationToken.None);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"{nameof(LoginAsync)}: " +
                $"Error retrieving trying to log in with email: {email}");

            return null;
        }
        var result = await response.Content.ReadFromJsonAsync<TokenModel>();
        
        return result;
    }

    /// <inheritdoc />
    public async Task<bool> IsCloseToExpireAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, FrontendConstants.TokenCloseToExpireApiEndpointRoute);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _resiliencePipeline.ExecuteAsync(
            async ct => await _httpClient.SendAsync(request, ct),
            CancellationToken.None);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"{nameof(IsCloseToExpireAsync)}: Error checking token expiry.");
            return true;
        }
        var result = await response.Content.ReadFromJsonAsync<bool>();

        return result!;
    }

    /// <inheritdoc />
    public async Task<TokenModel?> RefreshToken(TokenModel retrievedToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, FrontendConstants.TokenRefreshApiEndpointRoute)
        {
            Content = JsonContent.Create(retrievedToken)
        };

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", retrievedToken.AccessToken);
        var response = await _resiliencePipeline.ExecuteAsync(
            async ct => await _httpClient.SendAsync(request, ct),
            CancellationToken.None);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"{nameof(IsCloseToExpireAsync)}: Error refreshing the token.");
            return null;
        }
        var result = await response.Content.ReadFromJsonAsync<TokenModel>();

        return result!;
    }

    /// <inheritdoc />
    public async Task<UserInfo?> TryGetUserInfo(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, FrontendConstants.UserInfoApiEndpointRoute);
        request.Headers.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _resiliencePipeline.ExecuteAsync(
            async ct => await _httpClient.SendAsync(request, ct),
            CancellationToken.None);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"{nameof(TryGetUserInfo)}: Error retrieving user info.");
            return null;
        }
        var result = await response.Content.ReadFromJsonAsync<UserInfo>();
        
        return result!;
    }

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(string accessToken)
    {
        const string Empty = "{}";
        var emptyContent = new StringContent(Empty, Encoding.UTF8, MediaTypeNames.Application.Json);
        var request = new HttpRequestMessage(HttpMethod.Post, FrontendConstants.LogoutApiEndpointRoute)
        {
            Content = emptyContent
        };
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _resiliencePipeline.ExecuteAsync(
            async ct => await _httpClient.SendAsync(request, ct),
            CancellationToken.None);
        if (response.IsSuccessStatusCode) 
            return true;

        _logger.LogError($"{nameof(LogoutAsync)}: Error trying to log out");

        return false;
    }
}