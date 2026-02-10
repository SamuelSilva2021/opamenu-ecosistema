using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class CollaboratorRepository(OpamenuDbContext context) : OpamenuRepository<CollaboratorEntity>(context), ICollaboratorRepository
{
    private readonly OpamenuDbContext _context = context;

    public async Task<IEnumerable<CollaboratorEntity>> GetActiveByTenantIdAsync(Guid tenantId)
    {
        return await _context.Collaborators
            .Where(c => c.TenantId == tenantId && c.Active)
            .ToListAsync();
    }

    public async Task<CollaboratorEntity?> GetByUserAccountIdAsync(Guid userAccountId)
    {
        return await _context.Collaborators
            .FirstOrDefaultAsync(c => c.UserAccountId == userAccountId);
    }
}
