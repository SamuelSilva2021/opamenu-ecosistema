using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.Interfaces
{
    /// <summary>
    /// Define métodos específicos para o repositório de clientes de tenants aqui
    /// </summary>
    public interface ITenantCustomerRepository
    {
        Task<IEnumerable<TenantCustomerEntity>> GetByTenantIdAsync(Guid tenantId);
        Task<(IEnumerable<TenantCustomerEntity> Items, int TotalCount)> GetPagedByTenantIdAsync(Guid tenantId, string? search, int page, int limit);
        Task<TenantCustomerEntity?> GetByTenantIdAndCustomerIdAsync(Guid tenantId, Guid customerId);
        Task<TenantCustomerEntity?> CreateAsync(TenantCustomerEntity entity);
        Task<TenantCustomerEntity?> GetByTenantIdAndCustomerPhoneAsync(Guid tenantId, string phoneNumber);
    }
}

