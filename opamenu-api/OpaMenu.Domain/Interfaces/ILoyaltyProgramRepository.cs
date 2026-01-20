using System;
using System.Threading.Tasks;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface ILoyaltyProgramRepository : IRepository<LoyaltyProgramEntity>
{
    Task<LoyaltyProgramEntity?> GetByTenantIdAsync(Guid tenantId);
}
