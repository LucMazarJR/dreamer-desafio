using dedg_back.Data;
using dedg_back.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITimeEventService, TimeEventService>();
builder.Services.AddScoped<IMonthlyPeriodService, MonthlyPeriodService>();

var app = builder.Build();

// Apply migrations on startup with retry
var retryCount = 0;
var maxRetries = 10;
var delay = 2000; // 2 seconds

while (retryCount < maxRetries)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }
        break; // Success, exit loop
    }
    catch (Exception ex)
    {
        retryCount++;
        if (retryCount >= maxRetries)
        {
            Console.WriteLine($"Failed to apply migrations after {maxRetries} attempts: {ex.Message}");
            throw;
        }
        Console.WriteLine($"Attempting to apply migrations... (attempt {retryCount}/{maxRetries})");
        await Task.Delay(delay);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
