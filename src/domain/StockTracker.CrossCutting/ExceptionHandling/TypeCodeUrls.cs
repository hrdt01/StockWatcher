namespace StockTracker.CrossCutting.ExceptionHandling;

/// <summary>
/// Collection of URI refrences[RFC9110] that identifies the problem type. This specification encourages that,
/// when dereferenced, it provide human-readable documentation for the problem type.
/// https://datatracker.ietf.org/doc/html/rfc9110
/// </summary>
public static class TypeCodeUrls
{
    public const string BadRequestUrl = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    public const string UnauthorizedUrl = "https://tools.ietf.org/html/rfc9110#section-15.5.2";
    public const string InternalServerErrorUrl = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1";
    public const string NotFoundUrl = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5";
}