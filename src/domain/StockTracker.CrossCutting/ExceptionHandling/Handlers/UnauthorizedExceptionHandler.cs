using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

namespace StockTracker.CrossCutting.ExceptionHandling.Handlers;

/// <summary>
/// Unauthorized Exception Handler. It captures <see cref="UnauthorizedException"/>
/// </summary>
public class UnauthorizedExceptionExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;

    public UnauthorizedExceptionExceptionHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }


    /// <summary>
    /// Captures <see cref="UnauthorizedException"/> and wraps it in
    /// <see cref="ProblemDetails"/> object.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UnauthorizedException unauthorizedException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails(unauthorizedException);
        httpContext.Response.StatusCode = unauthorizedException.Status;

        if (_environment.IsDevelopment())
        {
            problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            problemDetails.StackTrace = unauthorizedException.StackTrace;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
