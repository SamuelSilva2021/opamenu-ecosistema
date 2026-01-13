
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface IAddonGroupRepository : IRepository<AddonGroupEntity>
{
    Task<IEnumerable<AddonGroupEntity>> GetActiveAddonGroupsAsync();
    Task<IEnumerable<AddonGroupEntity>> GetByTenantIdAsync(Guid tenantId);
    Task<IEnumerable<AddonGroupEntity>> GetActiveByTenantIdAsync(Guid tenantId);
    Task<AddonGroupEntity?> GetWithAddonsAsync(int id);
    Task<IEnumerable<AddonGroupEntity>> GetByProductIdAsync(int productId);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, Guid? tenantId = null);
    Task<int> GetNextDisplayOrderAsync(Guid? tenantId = null);
    Task UpdateDisplayOrdersAsync(Dictionary<int, int> groupDisplayOrders);
    Task<bool> HasAddonsAsync(int addonGroupId);
}
