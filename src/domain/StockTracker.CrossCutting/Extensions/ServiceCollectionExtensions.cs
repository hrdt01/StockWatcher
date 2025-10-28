using Microsoft.Extensions.DependencyInjection;
using StockTracker.CrossCutting.ExceptionHandling.Handlers;

namespace StockTracker.CrossCutting.Extensions;

/// <summary>
/// Extension method to Add custom exception handlers to service collection DI container.
/// The default exception handler MUST go at the end to catch any exception that has no custom exception created
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<RequestValidationExceptionHandler>();
        services.AddExceptionHandler<UnauthorizedExceptionExceptionHandler>();
        //services.AddExceptionHandler<NotFoundExceptionHandler>();

        services.AddExceptionHandler<DefaultExceptionHandler>();

        return services;
    }
}
