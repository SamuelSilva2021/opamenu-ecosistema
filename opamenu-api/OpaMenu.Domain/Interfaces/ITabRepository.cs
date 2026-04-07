using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface ITabRepository : IRepository<TabEntity>
{
    Task<IEnumerable<TabEntity>> GetByTableIdAsync(Guid tenantId, Guid tableId, ETabStatus? status = null);
    Task<int> CountByTableIdAsync(Guid tenantId, Guid tableId);
    Task<TabEntity?> GetFullTabByIdAndTableIdAsync(Guid tenantId, Guid tableId, Guid tabId);
}

