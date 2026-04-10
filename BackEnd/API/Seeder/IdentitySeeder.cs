using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Seeder
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(
     UserManager<ApplicationUser> userManager,
     RoleManager<IdentityRole<Guid>> roleManager,
     IConfiguration configuration)
        {
            var adminEmail = configuration["SuperAdmin:Email"] ?? throw new Exception("AdminEmail is not configured.");
            var adminPassword = configuration["SuperAdmin:Password"] ?? throw new Exception("AdminPassword is not configured.");

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                if (!await roleManager.RoleExistsAsync("SuperAdmin"))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>("SuperAdmin"));
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create SuperAdmin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }

                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to add admin user to SuperAdmin role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}