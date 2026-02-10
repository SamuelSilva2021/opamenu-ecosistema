using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface ICollaboratorRepository : IRepository<CollaboratorEntity>
{
    Task<IEnumerable<CollaboratorEntity>> GetActiveByTenantIdAsync(Guid tenantId);
    Task<CollaboratorEntity?> GetByUserAccountIdAsync(Guid userAccountId);
}
