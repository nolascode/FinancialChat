using System.Text;
using System.Text.Json;
using FinancialChat.API.Hubs;
using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Interfaces.Hubs;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Core.Settings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FinancialChat.API.Services;

public class StockResponseConsumer : BackgroundService
{
    private readonly ILogger<StockResponseConsumer> _logger;
    private readonly RabbitMqSettings _settings;
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IModel? _channel;

    public StockResponseConsumer(
        ILogger<StockResponseConsumer> logger,
        IOptions<RabbitMqSettings> settings,
        IHubContext<ChatHub, IChatClient> hubContext,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _settings = settings.Value;
        _hubContext = hubContext;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Response Consumer starting...");

        await Task.Delay(5000, stoppingToken); // Wait for RabbitMQ to be ready

        try
        {
            InitializeRabbitMq();
            StartConsuming();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Stock Response Consumer");
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

        _channel.QueueDeclare(
            queue: _settings.StockResponsesQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _logger.LogInformation("Connected to RabbitMQ and listening on {Queue}", _settings.StockResponsesQueue);
    }

    private void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received stock response: {Json}", json);

                var quote = JsonSerializer.Deserialize<StockQuoteDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (quote != null)
                {
                    await ProcessStockResponse(quote);
                }

                _channel?.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock response");
                _channel?.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(
            queue: _settings.StockResponsesQueue,
            autoAck: false,
            consumer: consumer);
    }

    private async Task ProcessStockResponse(StockQuoteDto quote)
    {
        // Save bot message to database
        using var scope = _scopeFactory.CreateScope();
        var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = quote.Message,
            Timestamp = DateTime.UtcNow,
            UserName = "StockBot",
            UserId = null,
            ChatRoomId = quote.ChatRoomId,
            IsBot = true
        };

        await repositoryManager.MessageRepository.CreateAsync(message);
        await repositoryManager.SaveAsync();

        // Broadcast to chat room via SignalR
        var messageDto = new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            Timestamp = message.Timestamp,
            UserName = "StockBot",
            IsBot = true,
            ChatRoomId = quote.ChatRoomId
        };

        await _hubContext.Clients.Group(quote.ChatRoomId.ToString()).ReceiveMessage(messageDto);
        await _hubContext.Clients.Group(quote.ChatRoomId.ToString()).ReceiveStockQuote(quote);

        _logger.LogInformation("Stock quote broadcasted to room {RoomId}: {Message}", quote.ChatRoomId, quote.Message);
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
