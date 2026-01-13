using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    /// <summary>
    /// Serviço para gerenciamento de tenants (clientes/empresas)
    /// </summary>
    public interface ITenantService
    {
        Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant);
    }
}
