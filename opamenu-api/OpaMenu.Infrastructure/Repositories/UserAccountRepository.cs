using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;

namespace OpaMenu.Infrastructure.Repositories;

public sealed class UserAccountRepository(AccessControlDbContext dbContext) : IUserAccountRepository
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<UserAccountEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.UserAccounts
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => u.DeletedAt == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u =>
                u.Username.Contains(s) ||
                u.Email.Contains(s) ||
                u.FirstName.Contains(s) ||
                u.LastName.Contains(s));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(IReadOnlyList<UserAccountEntity> Items, int Total)> GetPagedForTenantAsync(Guid tenantId, int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.UserAccounts
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => u.DeletedAt == null && u.TenantId == tenantId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u =>
                u.Username.Contains(s) ||
                u.Email.Contains(s) ||
                u.FirstName.Contains(s) ||
                u.LastName.Contains(s));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IReadOnlyList<UserAccountEntity>> GetActiveAsync()
    {
        return await _dbContext.UserAccounts
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => u.DeletedAt == null && u.Status == EUserAccountStatus.Ativo)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync();
    }

    public Task<UserAccountEntity?> GetByIdWithRoleAsync(Guid id) =>
        _dbContext.UserAccounts.AsNoTracking().Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

    public Task<UserAccountEntity?> GetByIdForTenantWithRoleAsync(Guid tenantId, Guid id) =>
        _dbContext.UserAccounts.AsNoTracking().Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null && u.TenantId == tenantId);

    public Task<UserAccountEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbContext.UserAccounts.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

    public Task<UserAccountEntity?> GetByIdForTenantTrackedAsync(Guid tenantId, Guid id) =>
        _dbContext.UserAccounts.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null && u.TenantId == tenantId);

    public Task<UserAccountEntity?> GetByEmailTrackedAsync(string email) =>
        _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.DeletedAt == null);

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null)
    {
        var query = _dbContext.UserAccounts.AsNoTracking().Where(u => u.Email.ToLower() == email && u.DeletedAt == null);
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null)
    {
        var query = _dbContext.UserAccounts.AsNoTracking().Where(u => u.Username == username && u.DeletedAt == null);
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }
        return await query.AnyAsync();
    }

    public Task<bool> ExistsAsync(Guid id) =>
        _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Id == id && u.DeletedAt == null);

    public Task AddAsync(UserAccountEntity entity)
    {
        _dbContext.UserAccounts.Add(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}

