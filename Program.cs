using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyFirstApi.Services;
using MyFirstApi.Services.Interfaces;
using MyFirstApi.Commands;
using MyFirstApi.Data.Seeders;
using QuestPDF.Infrastructure;
using MyFirstApi.Services.Background;


var builder = WebApplication.CreateBuilder(args);

// ========================================
// Services
// ========================================

// QuestPDF License
QuestPDF.Settings.License = LicenseType.Evaluation;
builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<EmployeeSeeder>();
builder.Services.AddScoped<SeedEmployeesCommand>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<EmailQueueService>();

builder.Services.AddSingleton<IEmailQueueService>(
    provider =>
        provider.GetRequiredService<EmailQueueService>()
);

builder.Services.AddHostedService<EmailBackgroundService>();



// ========================================
// Database
// ========================================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// ========================================
// JWT Authentication
// ========================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };

        options.Events = new JwtBearerEvents
        {
            // Token is missing
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        success = false,
                        message =
                            "Authorization token is required"
                    }
                );
            },

            // Token invalid / expired
            OnAuthenticationFailed = async context =>
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType =
                    "application/json";

                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        success = false,
                        message =
                            "Invalid or expired token"
                    }
                );
            }
        };
    });


// ========================================
// Seeder Command
// ========================================

if (args.Length > 0 &&
    args[0].Equals(
        "seed:employees",
        StringComparison.OrdinalIgnoreCase))
{
    var count = 1000;

    if (args.Length > 1 &&
        int.TryParse(args[1], out var parsedCount))
    {
        count = parsedCount;
    }

    var commandApp = builder.Build();

    using var scope =
        commandApp.Services.CreateScope();

    var command =
        scope.ServiceProvider
            .GetRequiredService<SeedEmployeesCommand>();

    await command.ExecuteAsync(count);

    return;
}


// ========================================
// Build Application
// ========================================

var app = builder.Build();


// ========================================
// Middleware
// ========================================

app.UseAuthentication();

app.UseAuthorization();


// ========================================
// Controllers
// ========================================

app.MapControllers();


// ========================================
// Run
// ========================================

app.Run();