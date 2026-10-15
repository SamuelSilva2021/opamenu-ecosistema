using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ITableRepository : IRepository<TableEntity>
{
    Task<TableEntity?> GetByNameAsync(string name, Guid tenantId);
    Task<bool> ExistsByNameAsync(string name, Guid tenantId);
    Task<IEnumerable<TableEntity>> GetPagedByTenantIdAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<TableEntity?> GetByIdWithDetailsAsync(Guid tenantId, Guid tableId);
    Task<IEnumerable<TableEntity>> GetPagedWithTabsAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<int> CountByTenantIdAsync(Guid tenantId);
}

