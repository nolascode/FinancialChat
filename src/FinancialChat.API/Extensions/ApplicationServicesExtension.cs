using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Interfaces.Hubs;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Core.Interfaces.Services;
using FinancialChat.Core.Settings;
using FinancialChat.Infrastructure.Messaging;
using FinancialChat.Infrastructure.Persistence.Context;
using FinancialChat.Infrastructure.Persistence.Repositories;
using FinancialChat.Infrastructure.Persistence.Seed;
using FinancialChat.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinancialChat.API.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<FinancialChatDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Identity
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<FinancialChatDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // For SignalR - extract token from query string
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });

        // Settings
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        services.Configure<StooqApiSettings>(configuration.GetSection("StooqApi"));

        // Repositories
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IMessageBrokerService, RabbitMqService>();

        // HTTP Client for Stock API
        services.AddHttpClient<IStockService, StockApiService>();

        // Service Manager - Centralizes all business logic services
        services.AddScoped<IServiceManager, ServiceManager>();

        // Database Seeder
        services.AddScoped<DatabaseSeeder>();

        // SignalR
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        })
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", builder =>
            {
                builder
                    .WithOrigins("http://localhost:5000", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // Razor Pages
        services.AddRazorPages();
        services.AddControllersWithViews();

        // Lowercase URLs
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        return services;
    }

    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating/seeding the database");
        }
    }
}
