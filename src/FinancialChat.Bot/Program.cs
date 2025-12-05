using FinancialChat.Bot.Workers;
using FinancialChat.Core.Interfaces.Services;
using FinancialChat.Core.Settings;
using FinancialChat.Infrastructure.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure settings
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.Configure<StooqApiSettings>(builder.Configuration.GetSection("StooqApi"));

// Register services
builder.Services.AddHttpClient<IStockService, StockApiService>();

// Register worker
builder.Services.AddHostedService<StockQuoteWorker>();

var host = builder.Build();
host.Run();
