using Common.Email; // Assuming MailSettings and FrontendSettings are in Common.Email or Entity
using Entity; // For FrontendSettings, WelcomeRequest (if defined here)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeopleStrong_API.Extensions; // For custom extensions like AddSwaggerDocumentation, AddApplicationServices, AddDatabaseConfiguration, AddJwtAuthentication
using Serilog;
using Services; // For AuthService, MailService, AttendanceService, EmployeeService
using Services.Interfaces; // For IAuthService, IMailService, IAttendanceService, IEmployeeService
using Data; // For ApplicationContext (if not already referenced)
using Microsoft.EntityFrameworkCore; // For UseMySql

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog FIRST for robust logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) // ?? Ensure this path is correct
    .CreateLogger();

// Configure MailSettings and FrontendSettings from appsettings.json
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));

// Register your services for Dependency Injection
builder.Services.AddTransient<IMailService, Services.MailService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IAuthService, AuthService>();
// Assuming you have an IEmployeeService and EmployeeService for onboarding/offboarding
//builder.Services.AddTransient<IEmployeeService, EmployeeService>();


// Tell the host to use Serilog
builder.Host.UseSerilog();

// Add controllers for API endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // For Swagger/OpenAPI

// Configure CORS policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", // Name your policy
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // ?? Your Angular frontend's exact origin
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Essential for sending cookies/JWT with credentials
        });
});

// Add custom extension methods for various configurations
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApplicationServices(); // Likely includes other repositories/services
builder.Services.AddDatabaseConfiguration(builder.Configuration); // Configures DbContext
builder.Services.AddJwtAuthentication(builder.Configuration); // Configures JWT authentication
builder.Services.AddTransient<GlobalExceptionHandler>(); // For global error handling

var app = builder.Build();

// Global exception handling middleware (should be very early in the pipeline)
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS

app.UseRouting(); // Enables attribute routing

// ?? NEW: Enable CORS middleware BEFORE UseAuthentication and UseAuthorization
app.UseCors("AllowSpecificOrigin"); // Use the policy you defined

app.UseAuthentication(); // ?? Authenticates the user based on the JWT token
app.UseAuthorization();  // ?? Authorizes the user based on roles/policies

app.MapControllers(); // Maps controller routes

// Example test log endpoint
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from logger at {time}", DateTime.UtcNow);
    return "Logging Works!";
});

app.Run();
