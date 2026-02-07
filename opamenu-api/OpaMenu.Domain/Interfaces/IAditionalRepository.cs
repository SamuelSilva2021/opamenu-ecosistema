using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface IAditionalRepository : IRepository<AditionalEntity>
{
    Task<IEnumerable<AditionalEntity>> GetAllAditionalsAsync(Guid tenantId);
    Task<IEnumerable<AditionalEntity>> GetByAditionalGroupIdAsync(Guid aditionalGroupId);
    Task<IEnumerable<AditionalEntity>> GetActiveByAditionalGroupIdAsync(Guid aditionalGroupId);
    Task<bool> IsNameUniqueInGroupAsync(string name, Guid aditionalGroupId, Guid? excludeId = null);
    Task<int> GetNextDisplayOrderAsync(Guid aditionalGroupId);
    Task UpdateDisplayOrdersAsync(Dictionary<int, int> aditionalDisplayOrders);
    Task<IEnumerable<AditionalEntity>> GetActiveAditionalsAsync();
}
