namespace StockTracker.Identity.Api.Areas.Identity.Models;

/// <summary>
/// User info from identity endpoint to establish claims.
/// </summary>
public record UserInfo()
{
    /// <summary>
    /// The email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The user name.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// A value indicating whether the email has been confirmed yet.
    /// </summary>
    public bool IsEmailConfirmed { get; set; }

    /// <summary>
    /// The list of claims for the user.
    /// </summary>
    public Dictionary<string, string> Claims { get; init; } = [];
}