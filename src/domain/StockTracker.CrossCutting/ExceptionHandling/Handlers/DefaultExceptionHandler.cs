using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace StockTracker.CrossCutting.ExceptionHandling.Handlers;


/// <summary>
/// Default Exception Handler. It captures any kind of exception that doesn't have a custom exception handler
/// </summary>
public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;

    public DefaultExceptionHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Captures  any non custom exception and wraps it in <see cref="ProblemDetails"/> object.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        var problemDetails = new ProblemDetails()
        {
            Title = "Bad Request",
            Detail = exception.Message,
            Type = TypeCodeUrls.BadRequestUrl,
            Status = httpContext.Response.StatusCode,
            TraceId = httpContext.TraceIdentifier
        };

        if (_environment.IsDevelopment())
        {
            problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            problemDetails.StackTrace = exception.StackTrace;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
