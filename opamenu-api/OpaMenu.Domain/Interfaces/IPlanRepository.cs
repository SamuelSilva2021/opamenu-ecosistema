using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpaMenu.Domain.Interfaces
{
    public interface IPlanRepository
    {
        Task<PlanEntity?> GetByIdAsync(Guid id);
        Task<PlanEntity?> GetByIdTrackedAsync(Guid id);
        Task<IEnumerable<PlanEntity>> GetAllActiveAsync();
        Task<(IReadOnlyList<PlanEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? name = null, string? status = null);
        Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
        Task AddAsync(PlanEntity entity);
        Task UpdateAsync(PlanEntity entity);
        Task DeleteAsync(PlanEntity entity);
        Task SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}

