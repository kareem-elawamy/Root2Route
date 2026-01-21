using Core;
using Domain.Models;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. التعديل الأول: إجبار التطبيق يشتغل HTTP فقط على بورت 5000
// ---------------------------------------------------------
builder.WebHost.UseUrls("http://localhost:5000");

#region Db Context Initialization
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

#endregion

#region Dependencies Initialization
builder.Services.AddServiceDependencies().AddServiceRegisteration().AddCoreDependencies().AddInfrastructureDependencies();
#endregion
//---------------------------------------------------------
// Identity Configuration is moved to ServiceRegisteration.cs
//---------------------------------------------------------

// Add services to the container.

// ---------------------------------------------------------
// 2. التعديل الثاني: لازم نضيف السطر ده عشان الـ API يشتغل
// ---------------------------------------------------------
builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ---------------------------------------------------------
// تم تعليق التوجيه لـ HTTPS عشان نمنع المشكلة
// ---------------------------------------------------------
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
