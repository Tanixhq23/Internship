using Common.Email;
using Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeopleStrong_API.Extensions;
using Serilog;
using Services;
using Services.Interfaces;

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

// Configure FrontendSettings from appsettings.json
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));

// Register IMailService with its concrete implementation
builder.Services.AddTransient<IMailService, Services.MailService>();

// Register IAttendanceService
builder.Services.AddTransient<IAttendanceService, AttendanceService>();

// Register IAuthService
builder.Services.AddTransient<IAuthService, AuthService>();

// Register IEmployeeService
builder.Services.AddTransient<IEmployeeService, EmployeeService>();

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

// ?? CRITICAL: Configure CORS policies for your frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", // Name your policy
        policyBuilder =>
        {
            // ?? Replace "http://localhost:4200" with your actual Angular frontend URL
            policyBuilder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Essential for sending cookies/JWT with credentials
        });
});


var app = builder.Build();
app.UseCors("AllowReactApp");


// Global exception handling middleware
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

app.UseRouting(); // Enables attribute routing

// ?? CRITICAL: Enable CORS middleware BEFORE UseAuthentication and UseAuthorization
app.UseCors("AllowSpecificOrigin"); // Use the policy you defined


app.UseAuthentication(); // Authenticates the user based on the JWT token
app.UseAuthorization();  // Authorizes the user based on roles/policies

app.MapControllers(); // Maps controller routes

// Example test log
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from logger at {time}", DateTime.UtcNow);
    return "Logging Works!";
});

// Run the app
app.Run();
