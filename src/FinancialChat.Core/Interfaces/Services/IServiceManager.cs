namespace FinancialChat.Core.Interfaces.Services;

/// <summary>
/// Defines a contract for managing application services.
/// Provides centralized access to all business logic services.
/// </summary>
public interface IServiceManager
{
    /// <summary>
    /// Gets the authentication service for user registration and login operations.
    /// </summary>
    IAuthService AuthService { get; }

    /// <summary>
    /// Gets the chat room service for managing chat rooms and messages.
    /// </summary>
    IChatRoomService ChatRoomService { get; }

    /// <summary>
    /// Gets the token service for JWT token generation and validation.
    /// </summary>
    ITokenService TokenService { get; }

    /// <summary>
    /// Gets the message broker service for RabbitMQ operations.
    /// </summary>
    IMessageBrokerService MessageBrokerService { get; }

    /// <summary>
    /// Gets the stock service for fetching stock quotes.
    /// </summary>
    IStockService StockService { get; }
}
