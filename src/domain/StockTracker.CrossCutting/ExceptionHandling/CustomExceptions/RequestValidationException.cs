using System.Net;

namespace StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

/// <inheritdoc />
[Serializable]
public class RequestValidationException : ProblemDetailsException
{
    public RequestValidationException()
    {
    }

    public RequestValidationException(string message) : base(message)
    {
        Title = "Request validation failed";
        Status = (int)HttpStatusCode.BadRequest;
        Type = TypeCodeUrls.BadRequestUrl;
    }

    public RequestValidationException(string message, Dictionary<string, List<string>> errors) : base(message)
    {
        Title = "Request validation failed";
        Status = (int)HttpStatusCode.BadRequest;
        Type = TypeCodeUrls.BadRequestUrl;
        Extensions = errors;
    }
}