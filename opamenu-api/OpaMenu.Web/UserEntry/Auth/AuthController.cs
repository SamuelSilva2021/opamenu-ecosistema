using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Auth;
using OpaMenu.Domain.DTOs.Auth;
using System.Security.Claims;

namespace OpaMenu.Web.UserEntry.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login-access-control")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAccessControl([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { error = "Refresh token is required" });
            }

            var result = await _authService.RefreshAsync(request.RefreshToken);
            
            if (!result.Succeeded || result.Data == null)
            {
                return Unauthorized(new { error = "Invalid refresh token" });
            }

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, new { error = "Internal server error during token refresh" });
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken);

        if (!result.Succeeded || result.Data == null)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.LogoutAsync(request.RefreshToken);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("user_id");
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var result = await _authService.GetMeAsync(userId, tenantSlug: null);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("permissions")]
    [Authorize]
    public async Task<IActionResult> Permissions([FromQuery] string? tenantSlug = null)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("user_id");
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var result = await _authService.GetMeAsync(userId, tenantSlug);
        if (!result.Succeeded || result.Data == null)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("validate")]
    [Authorize]
    public IActionResult Validate()
    {
        return Ok(true);
    }
}
