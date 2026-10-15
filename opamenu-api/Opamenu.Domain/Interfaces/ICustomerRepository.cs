using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opamenu.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<CustomerEntity>> GetByTenantIdAsync(Guid tenantId);
        Task<CustomerEntity?> GetByPhoneAsync(Guid tenantId, string phone);
        Task<CustomerEntity> CreateAsync(CustomerEntity entity);
        Task<CustomerEntity?> UpdateAsync(CustomerEntity entity);
        Task<CustomerEntity?> GetByIdAsync(Guid id);
        Task<CustomerEntity?> GetByEmailAsync(Guid tenantId, string email);
    }
}

