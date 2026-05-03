using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Repositories
{
    public class PlanRepository(MultiTenantDbContext context) : IPlanRepository
    {
        private readonly MultiTenantDbContext _context = context;

        public async Task<PlanEntity?> GetByIdAsync(Guid id) => await _context.Set<PlanEntity>().FindAsync(id);

        public async Task<PlanEntity?> GetByIdTrackedAsync(Guid id) => await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<PlanEntity>> GetAllActiveAsync() =>
            await _context.Set<PlanEntity>()
                .Where(p => p.Status == EPlanStatus.Ativo)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

        public async Task<(IReadOnlyList<PlanEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? name = null, string? status = null)
        {
            var query = _context.Plans.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var n = name.Trim();
                query = query.Where(p => p.Name.Contains(n) || p.Slug.Contains(n));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<EPlanStatus>(status, ignoreCase: true, out var st))
                {
                    query = query.Where(p => p.Status == st);
                }
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null) =>
            await _context.Plans.AnyAsync(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value));

        public async Task AddAsync(PlanEntity entity) => 
            await _context.Plans.AddAsync(entity);

        public async Task UpdateAsync(PlanEntity entity)
        {
            _context.Plans.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(PlanEntity entity)
        {
            _context.Plans.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<IDbContextTransaction> BeginTransactionAsync() =>
            await _context.Database.BeginTransactionAsync();
    }
}

