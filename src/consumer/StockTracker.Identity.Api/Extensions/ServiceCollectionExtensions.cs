using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Retry;
using StockTracker.CrossCutting.Constants;
using StockTracker.Identity.Api.Areas.Identity.Data;
using StockTracker.Identity.Api.Areas.Identity.Services;
using StockTracker.Identity.Api.Areas.Tracker.Services;
using StockTracker.Identity.Api.Data;
using StockTracker.Identity.Api.Migrations;
using System.Text;

namespace StockTracker.Identity.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void InitialIdentitySeeder(this WebApplication app, ConfigurationManager configurationInstance)
    {
        // Ensure the database is created
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<StockTrackerIdentityDbContext>();

            // Apply pending migrations
            dbContext.Database.Migrate();

            // Seed roles and admin user
            IdentitySeeder.SeedData(services, configurationInstance);
            dbContext.SaveChanges();
        }
    }

    public static IServiceCollection RegisterFunctionApiServices(
        this IServiceCollection services,
        ConfigurationManager configurationInstance)
    {
        var baseUrl = configurationInstance[ApiConstants.ApiBaseUrlSettingName];
        services.AddHttpClient(ApiConstants.FrontendClientFactoryName,
            client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

        services.AddResiliencePipeline(ApiConstants.FunctionResiliencePipelineName,
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
        services.AddScoped<ITrackerService, TrackerService>();
        return services;
    }
    public static IServiceCollection RegisterApiServices(
        this IServiceCollection services,
        ConfigurationManager configurationInstance, 
        string environmentApplicationName)
    {
        services.RegisterFunctionApiServices(configurationInstance);
        services.RegisterAuthenticationServices(configurationInstance);

        // Add a CORS policy for the client
        services.AddCors(
            options => options.AddPolicy(
                "wasm",
                policy => policy
                    .SetIsOriginAllowed(callerHost =>
                    {
                        var originToAllow = configurationInstance["ConsumerBaseUrl"];
                        return callerHost.StartsWith(originToAllow!);
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                )
            );

        services.AddScoped<ITokenService, TokenService>();

        services.ConfigureOpenTelemetry(configurationInstance, environmentApplicationName);

        return services;
    }

    private static IServiceCollection RegisterAuthenticationServices(
        this IServiceCollection services,
        ConfigurationManager configurationInstance)
    {
        var connectionString =
            configurationInstance.GetConnectionString("StockTrackerIdentityDbContextConnection") ??
            throw new InvalidOperationException("Connection string 'StockTrackerIdentityDbContextConnection' not found.");
        services.AddAuthorizationBuilder();
        services.AddDbContext<StockTrackerIdentityDbContext>(options => options.UseSqlite(connectionString));

        services
            .AddIdentity<StockTrackerUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<StockTrackerIdentityDbContext>()
            .AddDefaultTokenProviders()
            ;

        // Adding Authentication
        services
            .AddAuthorization()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configurationInstance["JWT:ValidAudience"],
                    ValidIssuer = configurationInstance["JWT:ValidIssuer"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationInstance["JWT:Secret"]))
                };
            })
            ;


        services.AddProblemDetails();
        services.AddHealthChecks();

        return services;
    }
}