using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.AccessControl;
using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Infrastructure.Repositories;

public sealed class AccessControlModuleRepository(AccessControlDbContext dbContext) : IAccessControlModuleRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<ModuleEntity>> GetActiveModulesWithKeyAsync()
    {
        return await _dbContext.Modules.AsNoTracking()
            .Where(m => m.IsActive && m.Key != null)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}
