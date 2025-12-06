using System.IdentityModel.Tokens.Jwt;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Settings;
using FinancialChat.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace FinancialChat.Tests.Unit.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            Key = "ThisIsAVerySecretKeyForTestingPurposesOnly123456!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        };

        var options = new Mock<IOptions<JwtSettings>>();
        options.Setup(x => x.Value).Returns(_jwtSettings);

        _tokenService = new TokenService(options.Object);
    }

    [Fact]
    public void GenerateJwtToken_WithValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Act
        var token = _tokenService.GenerateJwtToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Should().NotBeNull();
    }

    [Fact]
    public void GenerateJwtToken_ContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Act
        var token = _tokenService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Subject.Should().Be(userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == "test@example.com");
        jwtToken.Claims.Should().Contain(c => c.Type == "username" && c.Value == "testuser");
        jwtToken.Issuer.Should().Be(_jwtSettings.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtSettings.Audience);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com"
        };
        var token = _tokenService.GenerateJwtToken(user);

        // Act
        var isValid = _tokenService.ValidateToken(token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = _tokenService.ValidateToken(invalidToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ReturnsFalse()
    {
        // Act
        var isValid = _tokenService.ValidateToken(string.Empty);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GetUserIdFromToken_WithValidToken_ReturnsUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };
        var token = _tokenService.GenerateJwtToken(user);

        // Act
        var result = _tokenService.GetUserIdFromToken(token);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var result = _tokenService.GetUserIdFromToken(invalidToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GenerateJwtToken_TokenExpiresCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Act
        var token = _tokenService.GenerateJwtToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(5));
    }
}
