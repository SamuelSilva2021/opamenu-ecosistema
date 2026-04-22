using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpaMenu.Application.Services.Interfaces.Auth;
using OpaMenu.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OpaMenu.Infrastructure.Authentication;

public sealed class LocalAuthenticationService(
    IConfiguration configuration,
    ILogger<LocalAuthenticationService> logger,
    IAuthService authService) : IAuthenticationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<LocalAuthenticationService> _logger = logger;
    private readonly IAuthService _authService = authService;

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _configuration["Authentication:JwtSecret"];
            var jwtIssuer = _configuration["Authentication:JwtIssuer"];
            var jwtAudience = _configuration["Authentication:JwtAudience"];

            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                _logger.LogError("JWT Secret not configured");
                return Task.FromResult(false);
            }

            var key = Encoding.UTF8.GetBytes(jwtSecret);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
                ValidIssuer = jwtIssuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out _);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Task.FromResult(false);
        }
    }

    public Task<ClaimsPrincipal> GetClaimsFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            var claims = jsonToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");

            return Task.FromResult(new ClaimsPrincipal(identity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting claims from token");
            return Task.FromResult(new ClaimsPrincipal());
        }
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var result = await _authService.RefreshAsync(refreshToken);
        if (!result.Succeeded || result.Data == null)
        {
            return string.Empty;
        }

        return JsonSerializer.Serialize(result.Data);
    }
}

