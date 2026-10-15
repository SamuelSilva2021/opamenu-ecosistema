using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ICollaboratorRepository : IRepository<CollaboratorEntity>
{
    Task<IEnumerable<CollaboratorEntity>> GetActiveByTenantIdAsync(Guid tenantId);
    Task<CollaboratorEntity?> GetByUserAccountIdAsync(Guid userAccountId);
}
