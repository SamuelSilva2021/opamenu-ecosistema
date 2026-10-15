using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ITabRepository : IRepository<TabEntity>
{
    Task<IEnumerable<TabEntity>> GetByTableIdAsync(Guid tenantId, Guid tableId, ETabStatus? status = null);
    Task<int> CountByTableIdAsync(Guid tenantId, Guid tableId);
    Task<TabEntity?> GetFullTabByIdAndTableIdAsync(Guid tenantId, Guid tableId, Guid tabId);
}

