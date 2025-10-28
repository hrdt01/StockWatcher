using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StockTracker.Infrastructure.AzureTable.Definition;
using StockTracker.Infrastructure.AzureTable.Implementation;
using StockTracker.Infrastructure.AzureTable.Resolvers;
using StockTracker.Models.Persistence;
using System.ComponentModel.DataAnnotations;
using StockTracker.CrossCutting.Constants;
using StockTracker.CrossCutting.Extensions;
using StockTracker.Infrastructure.Auth.Definition;
using StockTracker.Infrastructure.Auth.Implementation;

namespace StockTracker.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add to the DI Service Collection the services needed to work 
    /// </summary>
    /// <param name="services">Current contract where the services will be added</param>
    /// <returns>Updated current contract</returns>
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddExceptionHandlers();
        services.AddConfigurationOptions<AzureTableOptions>(GlobalConstants.AzureTableRepositorySettingsSectionKeyName);
        services.AddConfigurationOptions<AzureQueueOptions>(GlobalConstants.AzureQueueRepositorySettingsSectionKeyName);

        services.AddSingleton<IAzureTableEntityResolver<StockInfoStorageTableKey>, StockInfoTableKeyResolver>();
        services.AddSingleton<IAzureTableEntityResolver<StockKpiStorageTableKey>, StockKpiTableKeyResolver>();
        services.AddSingleton<IAzureTableEntityResolver<TrackedCompanyStorageTableKey>, TrackedCompanyTableKeyResolver>();

        services.AddScoped<IStockInfoRepository, StockInfoRepository>();
        services.AddScoped<IStockKpiRepository, StockKpiRepository>();
        services.AddScoped<ITrackedCompanyRepository, TrackedCompanyRepository>();

        services.AddScoped<IKpiProcessorMessageBroker, KpiProcessorMessageBroker>();
        services.AddScoped<ICleanupProcessorMessageBroker, CleanupProcessorMessageBroker>();

        services.AddScoped<IConsumerAuthFlow, ConsumerAuthFlow>();

        return services;
    }


    public static IServiceCollection AddConfigurationOptions<TOptionSettings>(this IServiceCollection services, string sectionKey)
        where TOptionSettings : class, new()
    {
        services.AddOptions();
        services.AddSingleton((Func<IServiceProvider, IConfigureOptions<TOptionSettings>>)(p =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            var options = new ConfigureFromConfigurationOptions<TOptionSettings>(configuration.GetSection(sectionKey));

            if (options == null)
            {
                throw new NullReferenceException(nameof(options));
            }

            if (options.Action == null)
            {
                throw new NullReferenceException($"The configuration action from configuration options is NULL.");
            }

            var settings = new TOptionSettings();
            options.Action(settings);

            ValidateOptionSettings(settings);

            return options;
        }));

        return services;
    }

    private static void ValidateOptionSettings<TOptionSettings>(TOptionSettings settings) where TOptionSettings : class, new()
    {
        List<ValidationResult> list = new List<ValidationResult>();
        if (!Validator.TryValidateObject(settings, new ValidationContext(settings), list, validateAllProperties: true))
        {
            string text = string.Join(",", list.Select((ValidationResult x) => x.ErrorMessage)) ?? "";
            throw new ArgumentException("The " + typeof(TOptionSettings).Name + " section is invalid. " + text);
        }
    }
}