using FinancialChat.Core.DTOs.Chat;

namespace FinancialChat.Core.Interfaces.Services;

/// <summary>
/// Defines message broker operations for asynchronous communication between services.
/// </summary>
public interface IMessageBrokerService
{
    /// <summary>
    /// Publishes a stock quote request to the message queue.
    /// </summary>
    /// <param name="request">The stock request data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishStockRequestAsync(StockRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a stock quote response back to the application.
    /// </summary>
    /// <param name="response">The stock quote response data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishStockResponseAsync(StockQuoteDto response, CancellationToken cancellationToken = default);
}
