using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Infrastructure.Authentication;

/// <summary>
/// Serviço de autenticação externa que valida tokens JWT do saas-authentication-api
/// </summary>
public class ExternalAuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExternalAuthenticationService> _logger;
    private readonly HttpClient _httpClient;

    public ExternalAuthenticationService(
        IConfiguration configuration,
        ILogger<ExternalAuthenticationService> logger,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Get JWT settings from configuration
            var jwtSecret = _configuration["Authentication:JwtSecret"];
            var jwtIssuer = _configuration["Authentication:JwtIssuer"];
            var jwtAudience = _configuration["Authentication:JwtAudience"];

            if (string.IsNullOrEmpty(jwtSecret))
            {
                _logger.LogError("JWT Secret not configured");
                return Task.FromResult(false);
            }

            var key = Encoding.UTF8.GetBytes(jwtSecret);
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                ValidIssuer = jwtIssuer,
                ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
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
        try
        {
            var authUrl = _configuration["Authentication:ExternalAuthUrl"];
            if (string.IsNullOrEmpty(authUrl))
            {
                throw new InvalidOperationException("External Auth URL not configured");
            }

            var payload = new { refreshToken };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{authUrl}/auth/refresh-token", content);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to refresh token. Status: {StatusCode}", response.StatusCode);
                return string.Empty;
            }

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }
}
