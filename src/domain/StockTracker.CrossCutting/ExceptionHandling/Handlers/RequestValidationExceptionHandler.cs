using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;

namespace StockTracker.CrossCutting.ExceptionHandling.Handlers;

/// <summary>
/// RequestValidation Exception Handler. It captures <see cref="Utilities.ExceptionHandling.CustomExceptions.RequestValidationException"/>
/// </summary>
public class RequestValidationExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;

    public RequestValidationExceptionHandler(IHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Captures <see cref="RequestValidationException"/> and wraps it in
    /// <see cref="ProblemDetails"/> object.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not RequestValidationException validationException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails(validationException);
        httpContext.Response.StatusCode = validationException.Status;

        if (_environment.IsDevelopment())
        {
            problemDetails.Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            problemDetails.StackTrace = validationException.StackTrace;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}
