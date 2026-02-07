using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Informações básicas do usuário autenticado
/// </summary>
public class UserInfo
{
    /// <summary>
    /// ID do usuário
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome de usuário
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Permissões do usuário
    /// </summary>
    public UserPermissionsDTO Permissions { get; set; } = new UserPermissionsDTO();

    /// <summary>
    /// Perfil (Role) simplificado o usuário
    /// </summary>
    public SimplifiedRoleDTO? Role { get; set; }

    /// <summary>
    /// Informações do tenant (se aplicável)
    /// </summary>
    public TenantInfoDTO? Tenant { get; set; }
}