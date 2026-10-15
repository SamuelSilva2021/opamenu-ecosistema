using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

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
