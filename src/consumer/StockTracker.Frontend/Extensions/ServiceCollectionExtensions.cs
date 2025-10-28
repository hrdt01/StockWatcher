using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Polly;
using Polly.Retry;
using Radzen;
using StockTracker.Frontend.Constants;
using StockTracker.Frontend.Identity.Contracts.Definition;
using StockTracker.Frontend.Identity.Contracts.Implementation;
using StockTracker.Frontend.Services.Definition;
using StockTracker.Frontend.Services.Implementation;

namespace StockTracker.Frontend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterProjectDependencies(
        this IServiceCollection services,
        WebAssemblyHostBuilder hostBuilder)
    {
        // register the cookie handler
        services.AddTransient<CookieHandler>();
        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(hostBuilder.HostEnvironment.BaseAddress)
        });
        services.AddResiliencePipeline(FrontendConstants.ResiliencePipelineName,
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
        services.RegisterAuthenticationServices(hostBuilder);

        services.RegisterFrontendServices(hostBuilder);
        return services;
    }

    public static IServiceCollection RegisterFrontendServices(
        this IServiceCollection services,
        WebAssemblyHostBuilder hostBuilder)
    {
        var baseUrl = hostBuilder.Configuration[FrontendConstants.IdentityApiBaseUrlSettingName];
        services.AddHttpClient(FrontendConstants.FrontendClientFactoryName,
            client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });
        
        services.AddFluentUIComponents();
        services.AddRadzenComponents();
        services.AddScoped<IApiConsumer, ApiConsumer>();

        return services;
    }

    public static IServiceCollection RegisterAuthenticationServices(
        this IServiceCollection services,
        WebAssemblyHostBuilder hostBuilder)
    {
        services.AddSingleton<LocalStorageService>();
        //services.AddSingleton<UnauthorizedDelegatingHandler>();

        // set up authorization
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        // register the auth API consumer
        services.AddSingleton<IAuthApiConsumer, AuthApiConsumer>();
        
        services.AddSingleton<JwtAuthenticationStateProvider>();
        services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
        
        // register the account management interface
        services.AddSingleton(
            sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

        // configure client for auth interactions
        var baseUrl = hostBuilder.Configuration[FrontendConstants.IdentityApiBaseUrlSettingName];
        services.AddHttpClient(
                FrontendConstants.AuthClientFactoryName,
                opt => opt.BaseAddress = new Uri(baseUrl))
            .AddHttpMessageHandler<CookieHandler>()
            ;
        
        return services;
    }
}