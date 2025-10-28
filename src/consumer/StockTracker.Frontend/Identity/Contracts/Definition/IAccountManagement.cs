using StockTracker.Frontend.Identity.Models;

namespace StockTracker.Frontend.Identity.Contracts.Definition;

/// <summary>
/// Account management services.
/// </summary>
public interface IAccountManagement
{
    /// <summary>
    /// Login service.
    /// </summary>
    /// <param name="email">User's email.</param>
    /// <param name="password">User's password.</param>
    /// <returns>The result of the request serialized to <see cref="FormResult"/>.</returns>
    public Task<FormResult> LoginAsync(string email, string password);

    /// <summary>
    /// Log out the logged-in user.
    /// </summary>
    /// <returns>The asynchronous task.</returns>
    public Task LogoutAsync();

    public Task<bool> CheckAuthenticatedAsync();
}