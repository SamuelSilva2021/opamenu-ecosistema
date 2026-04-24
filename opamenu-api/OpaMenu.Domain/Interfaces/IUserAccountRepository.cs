using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;

namespace OpaMenu.Domain.Interfaces;

public interface IUserAccountRepository
{
    Task<(IReadOnlyList<UserAccountEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search);
    Task<(IReadOnlyList<UserAccountEntity> Items, int Total)> GetPagedForTenantAsync(Guid tenantId, int page, int limit, string? search);
    Task<IReadOnlyList<UserAccountEntity>> GetActiveAsync();
    Task<UserAccountEntity?> GetByIdWithRoleAsync(Guid id);
    Task<UserAccountEntity?> GetByIdForTenantWithRoleAsync(Guid tenantId, Guid id);
    Task<UserAccountEntity?> GetByIdTrackedAsync(Guid id);
    Task<UserAccountEntity?> GetByIdForTenantTrackedAsync(Guid tenantId, Guid id);
    Task<UserAccountEntity?> GetByEmailTrackedAsync(string email);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);
    Task<bool> ExistsAsync(Guid id);
    Task AddAsync(UserAccountEntity entity);
    Task SaveChangesAsync();
}

