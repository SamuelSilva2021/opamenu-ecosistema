using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace OpaMenu.Domain.Interfaces;

public interface ITenantRepository
{
    Task<TenantEntity?> GetBySlugAsync(string slug);
    Task<TenantEntity?> GetBySlugWithBusinessInfoAsync(string slug);
    Task<Guid> GetTenantIdBySlugAsyn(string slug);
    Task<TenantEntity?> GetByIdAsync(Guid id);
    Task UpdateAsync(TenantEntity entity);

    Task<(IReadOnlyList<TenantEntity> Items, int Total)> GetPagedAsync(
        int page,
        int limit,
        string? filterName,
        string? filterSlug,
        string? filterDomain,
        string? filterEmail,
        string? filterPhone,
        string? filterStatus);

    Task<TenantEntity?> GetByIdTrackedAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<bool> DocumentExistsAsync(string document);
    Task AddAsync(TenantEntity entity);
    Task SaveChangesAsync();
}
