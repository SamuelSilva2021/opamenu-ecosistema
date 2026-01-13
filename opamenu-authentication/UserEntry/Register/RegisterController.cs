using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using Authenticator.API.Infrastructure.Data;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Infrastructure.Data.Context;

namespace Authenticator.API.UserEntry.Register;

/// <summary>
/// Controller para registro de novos tenants (clientes/empresas)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RegisterController(
    MultiTenantDbContext tenantContext,
    AccessControlDbContext accessContext,
    IAuthenticationService authService,
    IJwtTokenService jwtTokenService,
    ITenantService tenantService,
    ILogger<RegisterController> logger
    ) : ControllerBase
{
    private readonly MultiTenantDbContext _tenantContext = tenantContext;
    private readonly AccessControlDbContext _accessContext = accessContext;
    private readonly IAuthenticationService _authService = authService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly ITenantService _tenantService = tenantService;
    private readonly ILogger<RegisterController> _logger = logger;

    /// <summary>
    /// Registra um novo tenant (cliente/empresa) com usuário administrador
    /// </summary>
    /// <param name="request">Dados do tenant e usuário administrador</param>
    /// <returns>InformaÃ§Ãµes do tenant criado com tokens de autenticaÃ§Ã£o</returns>
    [HttpPost]
    public async Task<ActionResult<ResponseDTO<RegisterTenantResponseDTO>>> Register([FromBody] CreateTenantDTO request)
    {
        var response = await _tenantService.AddTenantAsync(request);
        return StatusCode(response.Code, response);
    }
}

