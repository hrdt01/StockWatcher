using StockTracker.Identity.Api.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace StockTracker.Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.RegisterApiServices(builder.Configuration, builder.Environment.ApplicationName);

            builder.Services.AddControllers();  
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            // Activate the CORS policy
            app.UseCors("wasm");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true
            });

            app.UseStatusCodePages();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.InitialIdentitySeeder(builder.Configuration);
            app.Run();
        }
    }
}
