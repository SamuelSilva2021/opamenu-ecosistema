using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IAditionalGroupRepository : IRepository<AditionalGroupEntity>
{
    Task<IEnumerable<AditionalGroupEntity>> GetActiveAditionalGroupsAsync();
    Task<IEnumerable<AditionalGroupEntity>> GetByTenantIdAsync(Guid tenantId);
    Task<IEnumerable<AditionalGroupEntity>> GetActiveByTenantIdAsync(Guid tenantId);
    Task<AditionalGroupEntity?> GetWithAditionalsAsync(Guid id);
    Task<IEnumerable<AditionalGroupEntity>> GetByProductIdAsync(Guid productId);
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, Guid? tenantId = null);
    Task<int> GetNextDisplayOrderAsync(Guid? tenantId = null);
    Task UpdateDisplayOrdersAsync(Dictionary<int, int> groupDisplayOrders);
    Task<bool> HasAditionalsAsync(Guid aditionalGroupId);
}
