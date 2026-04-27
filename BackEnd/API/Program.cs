using API.Seeder;
using Core;
using Core.Filters;
using Domain.Constants;
using Domain.Models;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service;
using Service.Hubs;
using Service.Services.AuthenticationService;
using SixLabors.ImageSharp;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// ==================== CORS Config ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    // Use this policy for SignalR endpoints (credentials required)
    options.AddPolicy("SignalRPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) // Allow any origin with credentials
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// ==================== END CORS ====================

// Swagger Config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Root2Route API", Version = "v1" });
    c.EnableAnnotations();
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );
    c.OperationFilter<AddOrganizationHeaderFilter>();
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});



// Database Config
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
);

// ==================== JWT Config ====================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret))
{
    throw new Exception("JWT Settings are not configured correctly in appsettings.json");
}

builder.Services.AddSingleton(jwtSettings);

// ==================== END JWT ====================
builder.Services.AddLocalization();
// Dependencies
builder
    .Services.AddServiceDependencies()
    .AddServiceRegisteration()
    .AddCoreDependencies()
    .AddInfrastructureDependencies()
    .AddModelServiceDependencies(builder.Configuration);
builder.Services.AddSignalR();
// Authentication Config
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        };
    });

var app = builder.Build();
// Seed Identity Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await IdentitySeeder.SeedAsync(userManager, roleManager, builder.Configuration);
    }
    catch (Exception ex)
    {
        // Log the error (you can use any logging framework you prefer)
        Console.WriteLine($"Error seeding identity data: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Root2Route API v1"));
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<AuctionHub>("/hubs/auction").RequireCors("SignalRPolicy");

app.MapControllers();
app.MapGet("/", () => "Welcome to Root2Route API - System is Running Successfully! ");
app.Run();
