using System.Net;

namespace StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

/// <inheritdoc />
[Serializable]
public class UnauthorizedException : ProblemDetailsException
{
    public UnauthorizedException()
    {
    }
    public UnauthorizedException(string message) : base(message)
    {
        Title = "Unauthorized";
        Status = (int)HttpStatusCode.Unauthorized;
        Type = TypeCodeUrls.UnauthorizedUrl;
    }
    public UnauthorizedException(string message, Dictionary<string, List<string>> errors) : base(message)
    {
        Title = "Unauthorized";
        Status = (int)HttpStatusCode.Unauthorized;
        Type = TypeCodeUrls.UnauthorizedUrl;
        Extensions = errors;
    }
}
