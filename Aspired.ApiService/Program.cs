using Aspired.ApiService.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddSqlServerClient("aspired-sql");

builder.Services.AddDbContext<AspiredContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("aspired-db"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

var products = app.MapGroup("/products");

products.MapGet("/", async (AspiredContext dbContext) =>
{
    return await dbContext.Products
        .Where(x => x.IsEnabled)
        .ToListAsync();
});

products.MapGet("/all", async (AspiredContext dbContext) =>
{
    return await dbContext.Products
        .ToListAsync();
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
