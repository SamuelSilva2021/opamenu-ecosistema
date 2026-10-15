using Microsoft.EntityFrameworkCore;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Data.Context;
using Opamenu.Infrastructure.Shared.Data.Context.Opamenu;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Repositories;

public class CouponRepository(OpamenuDbContext context) : OpamenuRepository<CouponEntity>(context), ICouponRepository
{
    public async Task<IEnumerable<CouponEntity>> GetActiveCouponsAsync(Guid tenantId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(c => c.TenantId == tenantId && 
                        c.IsActive && 
                        (!c.StartDate.HasValue || c.StartDate <= now) &&
                        (!c.ExpirationDate.HasValue || c.ExpirationDate >= now) &&
                        (!c.UsageLimit.HasValue || c.UsageCount < c.UsageLimit))
            .ToListAsync();
    }

    public async Task<CouponEntity?> GetByCodeAsync(string code, Guid tenantId)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Code == code);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid tenantId, Guid? excludeId = null)
    {
        var query = _dbSet.Where(c => c.TenantId == tenantId && c.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }
        return !await query.AnyAsync();
    }
}

