using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockTracker.Identity.Api.Areas.Identity.Data;
using StockTracker.Identity.Api.Areas.Identity.Models;

namespace StockTracker.Identity.Api.Data;

public class StockTrackerIdentityDbContext : IdentityDbContext<StockTrackerUser, IdentityRole, string>
{
    public StockTrackerIdentityDbContext(DbContextOptions<StockTrackerIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
    public DbSet<TokenInfo> TokenInfos { get; set; }
}
