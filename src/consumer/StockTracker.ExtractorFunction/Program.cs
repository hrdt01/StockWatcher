using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockTracker.ExtractorFunction.Application.Extensions;
using StockTracker.ExtractorFunction.Extensions;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.RegisterFunctionServices();
builder.Services.AddApplicationArtifacts();
builder.Services.AddProblemDetails();

if (builder.Environment.IsProduction())
{
    builder.Services
        .AddApplicationInsightsTelemetryWorkerService();
    //.ConfigureFunctionsApplicationInsights();

    builder.Logging.Services.Configure<LoggerFilterOptions>(options =>
    {
        // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs. Application Insights requires an explicit override.
        // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/azure/azure-monitor/app/worker-service#ilogger-logs
        var defaultRule =
            options.Rules.FirstOrDefault(
                rule => rule.ProviderName ==
                        "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
        if (defaultRule is not null) options.Rules.Remove(defaultRule);
    });
}

var host = builder.Build();

host.Run();