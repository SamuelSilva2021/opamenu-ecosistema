using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Domain.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Authenticator.API.UserEntry.Auth;

/// <summary>
/// Controller de autenticação e autorização
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Tags("Autenticação")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authenticationService, ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Autentica um usuário e retorna o token JWT
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token JWT e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerResponse(200, "Login realizado com sucesso", typeof(ResponseDTO<LoginResponse>))]
    [SwaggerResponse(400, "Dados de entrada inválidos", typeof(ResponseDTO<LoginResponse>))]
    [SwaggerResponse(401, "Credenciais inválidas", typeof(ResponseDTO<LoginResponse>))]
    [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<LoginResponse>))]
    [ProducesResponseType(typeof(ResponseDTO<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<LoginResponse>), 400)]
    [ProducesResponseType(typeof(ResponseDTO<LoginResponse>), 401)]
    public async Task<ActionResult<ResponseDTO<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ResponseBuilder<LoginResponse>
                    .Fail(new ErrorDTO { Message = "Dados inválidos", Details = errors }).WithCode(400).Build();
            }

            var result = await _authenticationService.LoginAsync(request.UsernameOrEmail,request.Password);

            if (!result.Succeeded)
                return Unauthorized(result);

            _logger.LogInformation("Login realizado com sucesso para usuário: {Username}", request.UsernameOrEmail);
            return ResponseBuilder<LoginResponse>.Ok(result.Data!).Build();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login");
            return ResponseBuilder<LoginResponse>
                .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Renova o token de acesso usando o refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Novos tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ResponseDTO<LoginResponse>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<LoginResponse>), 400)]
    [ProducesResponseType(typeof(ResponseDTO<LoginResponse>), 401)]
    public async Task<ActionResult<ResponseDTO<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ResponseBuilder<LoginResponse>
                    .Fail(new ErrorDTO { Message = "Dados inválidos", Details = errors }).WithCode(400).Build();
            }

            var result = await _authenticationService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Succeeded)
                return Unauthorized(result);

            _logger.LogInformation("Token renovado com sucesso");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante renovação de token");
            return ResponseBuilder<LoginResponse>
                .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Realiza logout revogando o refresh token
    /// </summary>
    /// <param name="request">Refresh token para revogar</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseDTO<bool>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<bool>), 400)]
    public async Task<ActionResult<ResponseDTO<bool>>> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return ResponseBuilder<bool>
                   .Fail(new ErrorDTO { Message = "Dados inválidos", Details = errors }).WithCode(400).Build();
            }

            var result = await _authenticationService.RevokeTokenAsync(request.RefreshToken);

            if (!result.Succeeded)
                return BadRequest(result);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Logout realizado com sucesso para usuário: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return ResponseBuilder<bool>
                .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Busca as permissões do usuário autenticado
    /// </summary>
    /// <param name="tenantSlug">Slug do tenant (opcional)</param>
    /// <returns>Informações completas do usuário incluindo permissões</returns>
    [HttpGet("permissions")]
    [Authorize]
    [SwaggerResponse(200, "Permissões obtidas com sucesso", typeof(ResponseDTO<UserInfo>))]
    [SwaggerResponse(401, "Token JWT inválido ou expirado", typeof(ResponseDTO<UserInfo>))]
    [SwaggerResponse(404, "Usuário não encontrado", typeof(ResponseDTO<UserInfo>))]
    [SwaggerResponse(500, "Erro interno do servidor", typeof(ResponseDTO<UserInfo>))]
    [ProducesResponseType(typeof(ResponseDTO<UserInfo>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<UserInfo>), 401)]
    [ProducesResponseType(typeof(ResponseDTO<UserInfo>), 404)]
    public async Task<ActionResult<ResponseDTO<UserInfo>>> GetUserPermissions([FromQuery] string? tenantSlug = null)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _authenticationService.GetUserInfoAsync(userId, tenantSlug);

            if (!result.Succeeded)
                return NotFound(result);

            _logger.LogInformation("Permissões obtidas com sucesso para usuário: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter permissões do usuário");
            return ResponseBuilder<UserInfo>
                .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Valida se o token atual é válido
    /// </summary>
    /// <returns>Informações básicas do token</returns>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseDTO<object>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<object>), 401)]
    public ActionResult<ResponseDTO<object>> ValidateToken()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            var tenantSlugClaim = User.FindFirst("tenant_slug")?.Value;

            var tokenInfo = new
            {
                UserId = userIdClaim,
                Username = usernameClaim,
                Email = emailClaim,
                TenantId = tenantIdClaim,
                TenantSlug = tenantSlugClaim,
                IsValid = true,
                ValidatedAt = DateTime.UtcNow
            };

            return Ok(tokenInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar token");
            return ResponseBuilder<object>
               .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }

    /// <summary>
    /// Obtém informações básicas do usuário autenticado
    /// </summary>
    /// <returns>Informações do usuário</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseDTO<UserInfo>), 200)]
    [ProducesResponseType(typeof(ResponseDTO<UserInfo>), 401)]
    [ProducesResponseType(typeof(ResponseDTO<UserInfo>), 404)]
    public async Task<ActionResult<ResponseDTO<UserInfo>>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantSlugClaim = User.FindFirst("tenant_slug")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _authenticationService.GetUserInfoAsync(userId, tenantSlugClaim);

            if (!result.Succeeded)
                return NotFound();

            _logger.LogInformation("Informações do usuário obtidas com sucesso: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter informações do usuário");
            return ResponseBuilder<UserInfo>
               .Fail(new ErrorDTO { Message = "Erro interno do servidor" }).WithCode(500).Build();
        }
    }
}