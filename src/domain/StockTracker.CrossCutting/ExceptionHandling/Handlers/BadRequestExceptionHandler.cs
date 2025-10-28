using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

namespace StockTracker.CrossCutting.ExceptionHandling.Handlers;


/// <summary>
/// RequestValidation Exception Handler. It captures <see cref="BadRequestException"/>
/// </summary>
public class BadRequestExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;

    public BadRequestExceptionHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Captures <see cref="BadRequestException"/> and wraps it in <see cref="ProblemDetails"/> object.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadRequestException badRequestException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails(badRequestException);
        httpContext.Response.StatusCode = badRequestException.Status;

        if (_environment.IsDevelopment())
        {
            problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            problemDetails.StackTrace = badRequestException.StackTrace;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}