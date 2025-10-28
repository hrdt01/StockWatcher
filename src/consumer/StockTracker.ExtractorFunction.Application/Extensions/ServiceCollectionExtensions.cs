using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StockTracker.ExtractorFunction.Application.Contracts.Definition;
using StockTracker.ExtractorFunction.Application.Contracts.Implementation;
using StockTracker.ExtractorFunction.Application.Features.Behavior;
using StockTracker.MarketStack.Services.Extensions;

namespace StockTracker.ExtractorFunction.Application.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Method to add all StockTracker.ExtractorFunction related and required services
    /// </summary>
    public static IServiceCollection AddApplicationArtifacts(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>), ServiceLifetime.Scoped);
            cfg.AddOpenBehavior(typeof(QueryCachingBehavior<,>), ServiceLifetime.Scoped);
        });

        services.AddServicesExtensions();

        services.AddMediatRValidators();

        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }

    /// <summary>
    /// Extension method to add request validator in each feature to DI ServiceCollection container
    /// </summary>
    private static IServiceCollection AddMediatRValidators(this IServiceCollection services)
    {
        //Disable localization for FluentAssertions
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly, includeInternalTypes: true);

        return services;
    }
}