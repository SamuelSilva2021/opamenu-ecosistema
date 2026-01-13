using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Domain.Interfaces;

public interface ITableRepository : IRepository<TableEntity>
{
    Task<TableEntity?> GetByNameAsync(string name, Guid tenantId);
    Task<bool> ExistsByNameAsync(string name, Guid tenantId);
    Task<IEnumerable<TableEntity>> GetPagedByTenantIdAsync(Guid tenantId, int pageNumber, int pageSize);
    Task<int> CountByTenantIdAsync(Guid tenantId);
}

