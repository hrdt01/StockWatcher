using StockTracker.Frontend.Identity.Models;

namespace StockTracker.Frontend.Services.Definition;

public interface IAuthApiConsumer
{
    /// <summary>
    /// Login service.
    /// </summary>
    /// <param name="email">User's email.</param>
    /// <param name="password">User's password.</param>
    /// <returns>The result of the request serialized to <see cref="TokenModel"/>.</returns>
    Task<TokenModel?> LoginAsync(string email, string password);

    /// <summary>
    /// Log out the logged-in user.
    /// </summary>
    /// <param name="accessToken">Bearer access token.</param>
    /// <returns>The asynchronous task.</returns>
    public Task<bool> LogoutAsync(string accessToken);
    
    /// <summary>
    /// Tries to get information about the logged-in user.
    /// </summary>
    /// <param name="accessToken">Bearer access token.</param>
    /// <returns>The result of the request serialized to <see cref="UserInfo"/>.</returns>
    Task<UserInfo?> TryGetUserInfo(string accessToken);

    /// <summary>
    /// Tries to check if token for the logged-in user is close to expire.
    /// </summary>
    /// <param name="accessToken">Bearer access token.</param>
    /// <returns>The result of the request serialized to <see cref="UserInfo"/>.</returns>
    Task<bool> IsCloseToExpireAsync(string accessToken);

    /// <summary>
    /// Performs token refresh.
    /// </summary>
    /// <param name="retrievedToken"><see cref="TokenModel"/> instance.</param>
    /// <returns>New <see cref="TokenModel"/> instance.</returns>
    Task<TokenModel?> RefreshToken(TokenModel retrievedToken);
}