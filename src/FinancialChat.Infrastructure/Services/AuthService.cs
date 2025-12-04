using FinancialChat.Core.DTOs.Auth;
using FinancialChat.Core.Entities;
using FinancialChat.Core.Exceptions;
using FinancialChat.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace

namespace FinancialChat.Infrastructure.Services;

/// <summary>
/// Implements authentication service operations for user registration and login.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists");
        }

        var user = new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"Registration failed: {errors}");
        }

        _logger.LogInformation("User {UserName} registered successfully", user.UserName);

        var token = _tokenService.GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            UserId = user.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            Message = "Registration successful"
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email)
            ?? throw new BadRequestException("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            throw new BadRequestException("Invalid email or password");
        }

        _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

        var token = _tokenService.GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            UserId = user.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            Message = "Login successful"
        };
    }
}
