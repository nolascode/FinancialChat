using FinancialChat.API.Extensions;
using FinancialChat.API.Hubs;
using FinancialChat.API.Middlewares;
using FinancialChat.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddHostedService<StockResponseConsumer>();

var app = builder.Build();

// Apply migrations and seed data
await app.ApplyMigrationsAndSeedAsync();

// Configure pipeline
app.ConfigureExceptionHandler();

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/", () => Results.Redirect("/chat"));

app.Run();
