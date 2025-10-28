using System.Net;

namespace StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

/// <inheritdoc />
[Serializable]
public class BadRequestException : ProblemDetailsException
{

    public BadRequestException()
    {
    }

    public BadRequestException(string message) : base(message)
    {
        Title = "Bad Request";
        Status = (int)HttpStatusCode.BadRequest;
        Type = TypeCodeUrls.BadRequestUrl;
    }

    public BadRequestException(string message, Dictionary<string, List<string>> errors) : base(message)
    {
        Title = "Bad Request";
        Status = (int)HttpStatusCode.BadRequest;
        Type = TypeCodeUrls.BadRequestUrl;
        Extensions = errors;
    }
}