using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface IAddonRepository : IRepository<AddonEntity>
{
    Task<IEnumerable<AddonEntity>> GetAllAddonsAsync(Guid tenantId);
    Task<IEnumerable<AddonEntity>> GetByAddonGroupIdAsync(Guid addonGroupId);
    Task<IEnumerable<AddonEntity>> GetActiveByAddonGroupIdAsync(Guid addonGroupId);
    Task<bool> IsNameUniqueInGroupAsync(string name, Guid addonGroupId, Guid? excludeId = null);
    Task<int> GetNextDisplayOrderAsync(Guid addonGroupId);
    Task UpdateDisplayOrdersAsync(Dictionary<int, int> addonDisplayOrders);
    Task<IEnumerable<AddonEntity>> GetActiveAddonsAsync();
}
