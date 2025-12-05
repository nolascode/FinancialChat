using FinancialChat.Core.DTOs.Auth;
using FinancialChat.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialChat.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        var result = await _serviceManager.AuthService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _serviceManager.AuthService.LoginAsync(dto);
        return Ok(result);
    }
}
