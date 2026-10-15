using System.Collections.Generic;
using System.Threading.Tasks;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ILoyaltyProgramRepository : IRepository<LoyaltyProgramEntity>
{
    Task<IEnumerable<LoyaltyProgramEntity>> GetByTenantIdAsync(Guid tenantId);
}
