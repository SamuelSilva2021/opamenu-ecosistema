using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;

namespace OpaMenu.Infrastructure.Repositories;

public class TenantRepository(MultiTenantDbContext context) : MultiTenantRepository<TenantEntity>(context), ITenantRepository
{
    public async Task<TenantEntity?> GetBySlugAsync(string slug) => await _dbSet.FirstOrDefaultAsync(x => x.Slug == slug);

    public async Task<TenantEntity?> GetBySlugWithBusinessInfoAsync(string slug) => 
        await _dbSet.Include(x => x.BusinessInfo).Include(x => x.BankDetails).FirstOrDefaultAsync(x => x.Slug == slug);

    public async Task<Guid> GetTenantIdBySlugAsyn(string slug) => await _dbSet
            .Where(x => x.Slug == slug)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

    public async Task<TenantEntity?> GetByIdAsync(Guid id) => 
        await _dbSet.Include(x => x.BusinessInfo)
        .Include(x => x.BankDetails)
        .FirstOrDefaultAsync(x => x.Id == id);

    public async Task UpdateAsync(TenantEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<TenantEntity> Items, int Total)> GetPagedAsync(
        int page,
        int limit,
        string? filterName,
        string? filterSlug,
        string? filterDomain,
        string? filterEmail,
        string? filterPhone,
        string? filterStatus,
        string? filterType = null,
        Guid? filterParentTenantId = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filterName))
            query = query.Where(t => t.Name.Contains(filterName));

        if (!string.IsNullOrWhiteSpace(filterSlug))
            query = query.Where(t => t.Slug.Contains(filterSlug));

        if (!string.IsNullOrWhiteSpace(filterDomain))
            query = query.Where(t => t.Domain != null && t.Domain.Contains(filterDomain));

        if (!string.IsNullOrWhiteSpace(filterEmail))
            query = query.Where(t => t.Email != null && t.Email.Contains(filterEmail));

        if (!string.IsNullOrWhiteSpace(filterPhone))
            query = query.Where(t => t.Phone != null && t.Phone.Contains(filterPhone));

        if (!string.IsNullOrWhiteSpace(filterStatus))
            query = query.Where(t => t.Status.ToString() == filterStatus);

        if (!string.IsNullOrWhiteSpace(filterType))
            query = query.Where(t => t.Type.ToString() == filterType);

        if (filterParentTenantId.HasValue)
            query = query.Where(t => t.ParentTenantId == filterParentTenantId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public Task<TenantEntity?> GetByIdTrackedAsync(Guid id) =>
        _dbSet.FirstOrDefaultAsync(t => t.Id == id);

    public Task<bool> ExistsAsync(Guid id) =>
        _dbSet.AsNoTracking().AnyAsync(t => t.Id == id);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        var query = _dbSet.AsNoTracking().Where(t => t.Slug == slug);
        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> DocumentExistsAsync(string document) => await _dbSet.AsNoTracking().AnyAsync(t => t.Document == document);

    public Task AddAsync(TenantEntity entity)
    {
        _dbSet.Add(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

