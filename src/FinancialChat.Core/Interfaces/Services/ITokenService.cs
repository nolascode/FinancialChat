using FinancialChat.Core.Entities;

namespace FinancialChat.Core.Interfaces.Services;

/// <summary>
/// Defines service operations for JWT token management and validation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a new JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>A signed JWT token string.</returns>
    string GenerateJwtToken(User user);

    /// <summary>
    /// Extracts the user identifier from a JWT token.
    /// </summary>
    /// <param name="token">The JWT token string.</param>
    /// <returns>The user ID if found, otherwise null.</returns>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// Validates the structure and signature of a JWT token.
    /// </summary>
    /// <param name="token">The JWT token string.</param>
    /// <returns>True if the token is valid, otherwise false.</returns>
    bool ValidateToken(string token);
}
