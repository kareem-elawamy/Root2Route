using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Service.Abstract;
using Service.Implementations;

namespace Service
{
    public static class ServiceRegisteration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";



            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();
                services.AddScoped<IPlantInfoService, PlantInfoService>();
            return services;
        }
    }
}