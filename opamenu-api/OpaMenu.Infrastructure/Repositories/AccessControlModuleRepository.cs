using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

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
