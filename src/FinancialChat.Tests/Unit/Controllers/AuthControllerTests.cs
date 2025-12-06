using FinancialChat.API.Controllers;
using FinancialChat.Core.DTOs.Auth;
using FinancialChat.Core.Exceptions;
using FinancialChat.Core.Interfaces.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinancialChat.Tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _serviceManagerMock = new Mock<IServiceManager>();
        _authServiceMock = new Mock<IAuthService>();

        _serviceManagerMock
            .Setup(x => x.AuthService)
            .Returns(_authServiceMock.Object);

        _controller = new AuthController(_serviceManagerMock.Object);
    }

    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };

        var expectedResponse = new AuthResponseDto
        {
            Success = true,
            Token = "test-jwt-token",
            Expiration = DateTime.UtcNow.AddHours(1),
            UserId = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com",
            Message = "Registration successful"
        };

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        response.Success.Should().BeTrue();
        response.Token.Should().Be("test-jwt-token");
        response.UserName.Should().Be("testuser");
        response.Email.Should().Be("test@example.com");
        response.Message.Should().Be("Registration successful");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ThrowsConflictException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "testuser",
            Email = "existing@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException("User with this email already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _controller.Register(registerDto));
    }

    [Fact]
    public async Task Register_WhenCreateFails_ThrowsBadRequestException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "weak",
            ConfirmPassword = "weak"
        };

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Registration failed: Password too weak, Password must have uppercase"));

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _controller.Register(registerDto));
    }

    [Fact]
    public async Task Register_CallsAuthServiceWithCorrectDto()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponseDto { Success = true });

        // Act
        await _controller.Register(registerDto);

        // Assert
        _authServiceMock.Verify(x => x.RegisterAsync(registerDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ReturnsUserIdInResponse()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };

        var userId = Guid.NewGuid().ToString();
        var expectedResponse = new AuthResponseDto
        {
            Success = true,
            UserId = userId
        };

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        response.UserId.Should().NotBeNullOrEmpty();
        Guid.TryParse(response.UserId, out _).Should().BeTrue();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        var userId = Guid.NewGuid().ToString();
        var expectedResponse = new AuthResponseDto
        {
            Success = true,
            Token = "test-jwt-token",
            Expiration = DateTime.UtcNow.AddHours(1),
            UserId = userId,
            UserName = "testuser",
            Email = "test@example.com",
            Message = "Login successful"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        response.Success.Should().BeTrue();
        response.Token.Should().Be("test-jwt-token");
        response.UserName.Should().Be("testuser");
        response.Email.Should().Be("test@example.com");
        response.UserId.Should().Be(userId);
        response.Message.Should().Be("Login successful");
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ThrowsBadRequestException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Invalid email or password"));

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _controller.Login(loginDto));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ThrowsBadRequestException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Invalid email or password"));

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _controller.Login(loginDto));
    }

    [Fact]
    public async Task Login_CallsAuthServiceWithCorrectDto()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthResponseDto { Success = true });

        // Act
        await _controller.Login(loginDto);

        // Assert
        _authServiceMock.Verify(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_ReturnsExpirationTime()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123"
        };

        var expiration = DateTime.UtcNow.AddHours(1);
        var expectedResponse = new AuthResponseDto
        {
            Success = true,
            Expiration = expiration
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        response.Expiration.Should().NotBeNull();
        response.Expiration.Should().Be(expiration);
    }

    #endregion
}
