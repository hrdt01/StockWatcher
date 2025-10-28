using Microsoft.Extensions.DependencyInjection;
using StockTracker.CrossCutting.Constants;

namespace StockTracker.ExtractorFunction.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add to the DI Service Collection the services needed to work 
    /// </summary>
    /// <param name="services">Current contract where the services will be added</param>
    /// <returns>Updated current contract</returns>
    public static IServiceCollection RegisterFunctionServices(this IServiceCollection services)
    {
        services.AddHttpClient(ApiConstants.ClientFactoryName, httpClient =>
        {
            httpClient.BaseAddress = new Uri(ApiConstants.MarketStackBaseUrl);
        });

        services.AddMemoryCache();
        return services;
    }
}