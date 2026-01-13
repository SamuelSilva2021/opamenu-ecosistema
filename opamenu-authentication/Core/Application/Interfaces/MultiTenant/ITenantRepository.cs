using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace Authenticator.API.Core.Application.Interfaces.MultiTenant
{
    /// <summary>
    /// Repositório específico para operações relacionadas a tenants
    /// </summary>
    public interface ITenantRepository : IBaseRepository<TenantEntity>
    {
        /// <summary>
        /// Obtém um tenant pelo seu documento (CNPJ ou CPF)
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<TenantEntity?> GetByDocumentAsync(string document);
        /// <summary>
        /// Verifica se um slug já existe
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<bool> ExistingSlug(string slug);
    }
}

