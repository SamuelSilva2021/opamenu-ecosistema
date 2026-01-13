using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface IAddonRepository : IRepository<AddonEntity>
{
    Task<IEnumerable<AddonEntity>> GetAllAddonsAsync(Guid tenantId);
    Task<IEnumerable<AddonEntity>> GetByAddonGroupIdAsync(int addonGroupId);
    Task<IEnumerable<AddonEntity>> GetActiveByAddonGroupIdAsync(int addonGroupId);
    Task<bool> IsNameUniqueInGroupAsync(string name, int addonGroupId, int? excludeId = null);
    Task<int> GetNextDisplayOrderAsync(int addonGroupId);
    Task UpdateDisplayOrdersAsync(Dictionary<int, int> addonDisplayOrders);
    Task<IEnumerable<AddonEntity>> GetActiveAddonsAsync();
}
