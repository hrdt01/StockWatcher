using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockTracker.Identity.Api.Data;
using System.Data.Common;

namespace StockTracker.Identity.Api.IntegrationTests.Helpers;

public class IntegrationTestWebApplicationFactory<TProgram> : 
    WebApplicationFactory<TProgram> where TProgram : class
{
    public HttpClient HttpClient { get; private set; } = null!;

    public IntegrationTestWebApplicationFactory()
    {
        HttpClient = CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        }); 
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration(config =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            config.AddConfiguration(integrationConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            var dbContext = services.SingleOrDefault(x => 
                x.ServiceType == typeof(DbContextOptions<StockTrackerIdentityDbContext>));
            services.Remove(dbContext);

            var dbConnection = services.SingleOrDefault(x => 
                x.ServiceType == typeof(DbConnection));
            services.Remove(dbConnection);

            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });

            services.AddDbContext<StockTrackerIdentityDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }

}