using Microsoft.EntityFrameworkCore;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context.AccessControl;
using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Infrastructure.Repositories;

public sealed class GroupTypeRepository(AccessControlDbContext dbContext) : IGroupTypeRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<GroupTypeEntity>> GetAllAsync()
    {
        return await _dbContext.GroupTypes.AsNoTracking()
            .OrderBy(gt => gt.Name)
            .ToListAsync();
    }

    public Task<GroupTypeEntity?> GetByIdAsync(Guid id) =>
        _dbContext.GroupTypes.AsNoTracking().FirstOrDefaultAsync(gt => gt.Id == id);

    public Task<GroupTypeEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.GroupTypes.FirstOrDefaultAsync(gt => gt.Id == id);

    public Task<bool> ExistsAsync(Guid id) =>
        _dbContext.GroupTypes.AsNoTracking().AnyAsync(gt => gt.Id == id);

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _dbContext.GroupTypes.AsNoTracking().Where(gt => gt.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(gt => gt.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public Task AddAsync(GroupTypeEntity entity)
    {
        _dbContext.GroupTypes.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(GroupTypeEntity entity)
    {
        _dbContext.GroupTypes.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}

