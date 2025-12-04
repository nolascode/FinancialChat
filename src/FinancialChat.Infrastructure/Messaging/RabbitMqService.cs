using System.Text;
using System.Text.Json;
using FinancialChat.Core.DTOs.Chat;
using FinancialChat.Core.Interfaces.Services;
using FinancialChat.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FinancialChat.Infrastructure.Messaging;

public class RabbitMqService : IMessageBrokerService, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqService> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    public RabbitMqService(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        try
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

            _logger.LogInformation("RabbitMQ connection established. Queues declared: {RequestQueue}, {ResponseQueue}",
                _settings.StockRequestsQueue, _settings.StockResponsesQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
            throw;
        }
    }

    public Task PublishStockRequestAsync(StockRequestDto request, CancellationToken cancellationToken = default)
    {
        return PublishMessageAsync(_settings.StockRequestsQueue, request);
    }

    public Task PublishStockResponseAsync(StockQuoteDto response, CancellationToken cancellationToken = default)
    {
        return PublishMessageAsync(_settings.StockResponsesQueue, response);
    }

    private Task PublishMessageAsync<T>(string queueName, T message)
    {
        try
        {
            EnsureConnection();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel!.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            _logger.LogDebug("Published message to queue {Queue}: {Message}", queueName, json);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to queue {Queue}", queueName);
            throw;
        }
    }

    private void EnsureConnection()
    {
        if (_connection is null || !_connection.IsOpen)
        {
            _logger.LogWarning("RabbitMQ connection lost. Attempting to reconnect...");
            InitializeConnection();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        _disposed = true;
    }
}
