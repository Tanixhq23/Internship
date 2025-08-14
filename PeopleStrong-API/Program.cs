using Common.Email;
using Entity; // ?? Add this using statement for FrontendSettings
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeopleStrong_API.Extensions;
using Serilog;
using Services; // ?? Add this using statement for MailService if it's not already in scope

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173") // React dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


// Configure Serilog FIRST
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Configure MailSettings from appsettings.json
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// ?? Add configuration for FrontendSettings from appsettings.json
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));

// Register IMailService with its concrete implementation, passing IWebHostEnvironment
// ?? Updated registration to pass IWebHostEnvironment
builder.Services.AddTransient<IMailService, Services.MailService>();


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
app.UseCors("AllowReactApp");


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
