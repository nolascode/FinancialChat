namespace FinancialChat.Core.Settings;

/// <summary>
/// Configuration settings for RabbitMQ message broker connection and queues.
/// </summary>
public class RabbitMqSettings
{
    /// <summary>
    /// Gets or sets the hostname of the RabbitMQ server.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the port number of the RabbitMQ server.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Gets or sets the username for authentication.
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Gets or sets the password for authentication.
    /// </summary>
    public string Password { get; set; } = "guest";

    /// <summary>
    /// Gets or sets the name of the queue for stock quote requests.
    /// </summary>
    public string StockRequestsQueue { get; set; } = "stock_requests";

    /// <summary>
    /// Gets or sets the name of the queue for stock quote responses.
    /// </summary>
    public string StockResponsesQueue { get; set; } = "stock_responses";
}
