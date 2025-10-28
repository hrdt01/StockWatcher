using Microsoft.AspNetCore.Identity;
using StockTracker.CrossCutting.Constants;
using StockTracker.Identity.Api.Areas.Identity.Data;

namespace StockTracker.Identity.Api.Migrations;

public static class IdentitySeeder
{
    public static void SeedData(IServiceProvider serviceProvider, ConfigurationManager configurationInstance)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        SeedRoles(roleManager);

        var userManager = serviceProvider.GetRequiredService<UserManager<StockTrackerUser>>();
        SeedUsers(userManager, configurationInstance);
    }

    private static void SeedUsers(UserManager<StockTrackerUser> userManager, ConfigurationManager configurationInstance)
    { 
        // Define the admin user details
        var adminEmail = configurationInstance["AdminUserName"];
        var adminPassword = configurationInstance["AdminPassword"];

        AddUser(adminEmail,adminPassword, GlobalConstants.UserRoleAdmin, userManager);
        
        // Define the admin user details
        var viewerEmail = configurationInstance["ViewerUserName"];
        var viewerPassword = configurationInstance["ViewerPassword"];

        AddUser(viewerEmail, viewerPassword, GlobalConstants.UserRoleViewer, userManager);
    }

    private static void AddUser(string? userName, string? userPassword, string userRole, UserManager<StockTrackerUser> userManager)
    {
        // Check if the user already exists
        var userExist = userManager.FindByEmailAsync(userName).Result;
        if (userExist != null)
            return;

        var adminUser = new StockTrackerUser
        {
            UserName = userName.Split("@")[0],
            NormalizedUserName = $"StockTracker {userName.Split("@")[0]}",
            Email = userName,
            NormalizedEmail = userName,
            EmailConfirmed = true
        };

        // Create the admin user
        var result = userManager.CreateAsync(adminUser, userPassword).Result;
        if (!result.Succeeded)
            throw new Exception($"Failed to create the user: {userName}" + string.Join(", ", result.Errors));

        // Assign the Admin role to the user
        var addedToRoleResult = userManager.AddToRoleAsync(adminUser, userRole).Result;
    }

    private static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        // Define roles to seed
        var roles = new[] { GlobalConstants.UserRoleAdmin, GlobalConstants.UserRoleViewer };

        // Seed roles
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                var result = roleManager.CreateAsync(new IdentityRole(role)).Result;
            }
        }
    }
}