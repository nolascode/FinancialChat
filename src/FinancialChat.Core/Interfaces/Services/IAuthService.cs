using FinancialChat.Core.DTOs.Auth;

namespace FinancialChat.Core.Interfaces.Services;

/// <summary>
/// Defines authentication service operations for user registration and login.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    /// <param name="registerDto">The registration data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with token if successful.</returns>
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with token if successful.</returns>
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
}
