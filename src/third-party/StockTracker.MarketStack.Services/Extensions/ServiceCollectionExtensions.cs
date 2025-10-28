using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using StockTracker.CrossCutting.Constants;
using StockTracker.Infrastructure.Extensions;
using StockTracker.MarketStack.Services.Contracts.Definition;

namespace StockTracker.MarketStack.Services.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add to the DI Service Collection the services needed to work with the business logic services
    /// </summary>
    /// <param name="services">Current contract where the services will be added</param>
    /// <returns>Updated current contract</returns>
    public static IServiceCollection AddServicesExtensions(this IServiceCollection services)
    {
        services.RegisterInfrastructureServices();
        services.AddScoped<IStockTracker, Contracts.Implementation.StockTracker>();
        services.AddScoped<IStockKpiCalculator, Contracts.Implementation.StockKpiCalculator>();

        services.AddResiliencePipeline(ApiConstants.ResiliencePipelineName,
            builder =>
            {
                builder.AddRetry(new RetryStrategyOptions()
                {
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(5),
                    
                })
                .AddTimeout(TimeSpan.FromSeconds(30))
                ;
            });

        return services;
    }
}