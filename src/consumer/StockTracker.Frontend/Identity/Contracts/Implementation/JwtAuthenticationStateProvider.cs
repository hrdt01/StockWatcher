using Microsoft.AspNetCore.Components.Authorization;
using StockTracker.Frontend.Identity.Contracts.Definition;
using StockTracker.Frontend.Identity.Models;
using System.Net;
using System.Security.Claims;
using StockTracker.Frontend.Services.Definition;

namespace StockTracker.Frontend.Identity.Contracts.Implementation;

/// <summary>
/// Handles state for cookie-based auth.
/// </summary>
/// <remarks>
/// Create a new instance of the auth provider.
/// </remarks>
public class JwtAuthenticationStateProvider
        : AuthenticationStateProvider, IAccountManagement
{
    private const string LocalStorageKey = "currentUser";

    private readonly LocalStorageService _localStorageService;
    private readonly IAuthApiConsumer _authApiConsumer;
    private readonly ILogger<JwtAuthenticationStateProvider> _logger;

    /// <summary>
    /// Authentication state.
    /// </summary>
    private bool authenticated = false;

    /// <summary>
    /// Default principal for anonymous (not authenticated) users.
    /// </summary>
    private ClaimsPrincipal unauthenticated = new ClaimsPrincipal(new ClaimsIdentity());

    /// <summary>
    /// The token model retrieved from the login request.
    /// </summary>
    private TokenModel retrievedToken = null;

    public JwtAuthenticationStateProvider(
        IAuthApiConsumer authApiConsumer,
        LocalStorageService localStorageService,
        ILogger<JwtAuthenticationStateProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _authApiConsumer = authApiConsumer;
        _localStorageService = localStorageService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<FormResult> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation($"{nameof(LoginAsync)} execution for user: {email}");
            var result = await _authApiConsumer.LoginAsync(email, password);
           
            if (result != null)
            {
                _logger.LogInformation($"Login success for user: {email}");
                retrievedToken = result;
                await SetCurrentTokenAsync(retrievedToken);

                // need to refresh auth state
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                // success!
                return new FormResult { Succeeded = true };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(LoginAsync)}: App error");
        }

        // unknown error
        return new FormResult
        {
            Succeeded = false,
            ErrorList = ["Invalid email and/or password."]
        };
    }

    /// <summary>
    /// Get authentication state.
    /// </summary>
    /// <remarks>
    /// Called by Blazor anytime and authentication-based decision needs to be made, then cached
    /// until the changed state notification is raised.
    /// </remarks>
    /// <returns>The authentication state asynchronous request.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        authenticated = false;

        // default to not authenticated
        var user = unauthenticated;

        try
        {
            retrievedToken = await GetCurrentTokenAsync();

            if (retrievedToken == null)
            {
                return new AuthenticationState(user);
            }
            await CheckTokenExpirationAsync();
            
            var userInfo = await _authApiConsumer.TryGetUserInfo(retrievedToken.AccessToken);

            if (userInfo != null)
            {
                var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Username),
                        new(ClaimTypes.Email, userInfo.Email),
                    };

                // add any additional claims
                claims.AddRange(
                    userInfo.Claims.Where(
                            c => 
                                c.Key != ClaimTypes.Name 
                                 && c.Key != ClaimTypes.Email)
                        .Select(c => new Claim(c.Key, c.Value)));

                // set the principal
                var claimIdentity = new ClaimsIdentity(claims, nameof(JwtAuthenticationStateProvider));
                user = new ClaimsPrincipal(claimIdentity);
                authenticated = true;
            }
        }
        catch (Exception ex) when (ex is HttpRequestException exception)
        {
            if (exception.StatusCode != HttpStatusCode.Unauthorized)
            {
                _logger.LogError(ex, $"{nameof(GetAuthenticationStateAsync)}: App error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetAuthenticationStateAsync)}: App error");
        }

        // return the state
        return new AuthenticationState(user);
    }

    /// <inheritdoc />
    public async Task LogoutAsync()
    {
        var result = await _authApiConsumer.LogoutAsync(retrievedToken.AccessToken);
        if (result)
        {
            retrievedToken = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return authenticated;
    }

    public async Task SetCurrentTokenAsync(TokenModel? currentToken)
    {
        await _localStorageService.SetItem(LocalStorageKey, currentToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public Task<TokenModel?> GetCurrentTokenAsync() => _localStorageService.GetItemAsync<TokenModel>(LocalStorageKey);

    /// <summary>
    /// Check if the token is close to expire and refresh if needed.
    /// </summary>
    /// <returns></returns>
    private async Task CheckTokenExpirationAsync()
    {
        var isCloseToExpire = await _authApiConsumer.IsCloseToExpireAsync(retrievedToken.AccessToken);
        if (isCloseToExpire)
        {
            _logger.LogInformation("Access token is close to expire, logging out.");
            await RefreshAsync();
        }
    }

    /// <summary>
    /// Tries to refresh the token.
    /// </summary>
    /// <returns></returns>
    private async Task RefreshAsync()
    {
        var result = await _authApiConsumer.RefreshToken(retrievedToken);

        if (result != null)
        {
            _logger.LogInformation("Refresh token success");
            retrievedToken = result;

            // need to refresh auth state
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

}