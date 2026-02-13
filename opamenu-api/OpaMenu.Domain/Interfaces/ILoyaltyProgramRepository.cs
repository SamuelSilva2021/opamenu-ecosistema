using System.Collections.Generic;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface ILoyaltyProgramRepository : IRepository<LoyaltyProgramEntity>
{
    Task<IEnumerable<LoyaltyProgramEntity>> GetByTenantIdAsync(Guid tenantId);
}
