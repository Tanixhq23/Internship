using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeopleStrong_API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog FIRST
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Tell the host to use Serilog
builder.Host.UseSerilog();

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplicationServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddTransient<GlobalExceptionHandler>();

var app = builder.Build();

// Global exception handling middleware
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Example test log
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from logger at {time}", DateTime.UtcNow);
    return "Logging Works!";
});

// Run the app
app.Run();
