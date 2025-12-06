using System.Text;
using System.Text.Json;
using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Interfaces.Services;
using FinancialChat.Core.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FinancialChat.Bot.Workers;

public class StockQuoteWorker : BackgroundService
{
    private readonly ILogger<StockQuoteWorker> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly IStockService _stockService;
    private IConnection? _connection;
    private IModel? _channel;

    public StockQuoteWorker(
        ILogger<StockQuoteWorker> logger,
        IOptions<RabbitMqSettings> settings,
        IStockService stockService)
    {
        _logger = logger;
        _settings = settings.Value;
        _stockService = stockService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Quote Worker starting...");

        var retryCount = 0;
        const int maxRetries = 10;

        while (!stoppingToken.IsCancellationRequested && retryCount < maxRetries)
        {
            try
            {
                var delay = Math.Min(5000 * (int)Math.Pow(2, retryCount), 60000);
                _logger.LogInformation("Waiting {Delay}ms before connecting to RabbitMQ (attempt {Attempt}/{Max})",
                    delay, retryCount + 1, maxRetries);

                await Task.Delay(delay, stoppingToken);

                InitializeRabbitMq();
                StartConsuming(stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                retryCount++;
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ (attempt {Attempt}/{Max})",
                    retryCount, maxRetries);

                if (retryCount >= maxRetries)
                {
                    _logger.LogError("Max retries reached. Stock Quote Worker stopping.");
                }
            }
        }
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declare queues
        _channel.QueueDeclare(
            queue: _settings.StockRequestsQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueDeclare(
            queue: _settings.StockResponsesQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Set prefetch count to 1 to process one message at a time
        _channel.BasicQos(0, 1, false);

        _logger.LogInformation("Connected to RabbitMQ at {Host}:{Port}", _settings.HostName, _settings.Port);
    }

    private void StartConsuming(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            if (stoppingToken.IsCancellationRequested) return;

            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received stock request: {Json}", json);

                var request = JsonSerializer.Deserialize<StockRequestDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (request != null)
                {
                    await ProcessStockRequest(request);
                }

                _channel?.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock request");
                _channel?.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(
            queue: _settings.StockRequestsQueue,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Started consuming from {Queue}", _settings.StockRequestsQueue);
    }

    private async Task ProcessStockRequest(StockRequestDto request)
    {
        _logger.LogInformation("Processing stock request for {StockCode}", request.StockCode);

        try
        {
            var quote = await _stockService.GetStockQuoteAsync(request.StockCode);

            var response = new StockQuoteDto
            {
                StockCode = quote.StockCode,
                Price = quote.Price,
                Message = quote.Message,
                ChatRoomId = request.ChatRoomId,
                CorrelationId = request.CorrelationId,
                Success = quote.Success,
                Error = quote.Error
            };

            await PublishResponse(response);

            _logger.LogInformation("Processed stock request for {StockCode}: {Message}",
                request.StockCode, response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stock quote for {StockCode}", request.StockCode);

            var errorResponse = new StockQuoteDto
            {
                StockCode = request.StockCode.ToUpperInvariant(),
                Message = $"Error processing stock request for {request.StockCode.ToUpperInvariant()}: {ex.Message}",
                ChatRoomId = request.ChatRoomId,
                CorrelationId = request.CorrelationId,
                Success = false,
                Error = ex.Message
            };

            await PublishResponse(errorResponse);
        }
    }

    private Task PublishResponse(StockQuoteDto response)
    {
        var json = JsonSerializer.Serialize(response);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel!.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _settings.StockResponsesQueue,
            basicProperties: properties,
            body: body);

        _logger.LogDebug("Published response to {Queue}: {Json}", _settings.StockResponsesQueue, json);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        base.Dispose();
    }
}
