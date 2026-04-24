using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class ModuleRepository(AccessControlDbContext dbContext) : IModuleRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public Task<string?> GetKeyByIdAsync(Guid id) =>
        _dbContext.Modules.AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => m.Key)
            .FirstOrDefaultAsync();

    public Task<ModuleEntity?> GetByIdAsync(Guid id) =>
        _dbContext.Modules.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

    public async Task<(Guid Id, string Name)?> GetIdAndNameByKeyAsync(string key)
    {
        var module = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.Key != null && m.Key == key)
            .Select(m => new { m.Id, m.Name })
            .FirstOrDefaultAsync();

        return module == null ? null : (module.Id, module.Name);
    }

    public async Task<IReadOnlyList<(Guid Id, string Key, string Name)>> GetIdAndNameByKeysAsync(IReadOnlyCollection<string> keys)
    {
        if (keys.Count == 0)
        {
            return [];
        }

        var modules = await _dbContext.Modules.AsNoTracking()
            .Where(m => m.Key != null && keys.Contains(m.Key))
            .Select(m => new { m.Id, Key = m.Key!, m.Name })
            .ToListAsync();

        return modules.Select(m => (m.Id, m.Key, m.Name)).ToList();
    }
}

