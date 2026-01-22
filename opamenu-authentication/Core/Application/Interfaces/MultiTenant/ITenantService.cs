using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    /// <summary>
    /// Implementa as operações relacionadas a tenants (clientes/empresas)
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Adiciona um novo tenant (cliente/empresa) ao sistema
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant);
        /// <summary>
        /// Busca todos os tenants (clientes/empresas) com paginação e filtros opcionais
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseDTO<PagedResponseDTO<TenantSummaryDTO>>> GetAllAsync(int page, int limit, TenantFilterDTO? filter = null);
        /// <summary>
        /// Busca um tenant (cliente/empresa) pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<TenantDTO>> GetByIdAsync(Guid id);
        /// <summary>
        /// Atualiza os dados de um tenant (cliente/empresa)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ResponseDTO<TenantDTO>> UpdateAsync(Guid id, UpdateTenantDTO dto);
        /// <summary>
        /// Deleta um tenant (cliente/empresa) pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    }
}
