using FinancialChat.Core.Entities;
using FinancialChat.Core.Interfaces.Repositories;
using FinancialChat.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FinancialChat.Infrastructure.Services;

/// <summary>
/// Implements IServiceManager to coordinate application services
/// and handle business logic dependencies.
/// </summary>
public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IAuthService> _authService;
    private readonly Lazy<IChatRoomService> _chatRoomService;
    private readonly ITokenService _tokenService;
    private readonly IMessageBrokerService _messageBrokerService;
    private readonly IStockService _stockService;

    /// <summary>
    /// Initializes a new instance of the ServiceManager class.
    /// </summary>
    /// <param name="repositoryManager">The repository manager for data access.</param>
    /// <param name="userManager">The ASP.NET Identity user manager.</param>
    /// <param name="signInManager">The ASP.NET Identity sign-in manager.</param>
    /// <param name="tokenService">The JWT token service.</param>
    /// <param name="messageBrokerService">The message broker service for RabbitMQ.</param>
    /// <param name="stockService">The stock quote service.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    public ServiceManager(
        IRepositoryManager repositoryManager,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IMessageBrokerService messageBrokerService,
        IStockService stockService,
        ILoggerFactory loggerFactory)
    {
        _tokenService = tokenService;
        _messageBrokerService = messageBrokerService;
        _stockService = stockService;

        // Lazy initialization for services that depend on repository manager
        _authService = new Lazy<IAuthService>(() => new AuthService(
            userManager,
            signInManager,
            tokenService,
            loggerFactory.CreateLogger<AuthService>()));

        _chatRoomService = new Lazy<IChatRoomService>(() => new ChatRoomService(
            repositoryManager,
            loggerFactory.CreateLogger<ChatRoomService>()));
    }

    /// <inheritdoc />
    public IAuthService AuthService => _authService.Value;

    /// <inheritdoc />
    public IChatRoomService ChatRoomService => _chatRoomService.Value;

    /// <inheritdoc />
    public ITokenService TokenService => _tokenService;

    /// <inheritdoc />
    public IMessageBrokerService MessageBrokerService => _messageBrokerService;

    /// <inheritdoc />
    public IStockService StockService => _stockService;
}
