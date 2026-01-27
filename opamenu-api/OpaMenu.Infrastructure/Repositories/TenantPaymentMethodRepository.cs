using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class TenantPaymentMethodRepository(OpamenuDbContext context) : BaseRepository<TenantPaymentMethodEntity>(context), ITenantPaymentMethodRepository
{
    public async Task<IEnumerable<TenantPaymentMethodEntity>> GetAllByTenantWithPaymentMethodAsync(Guid tenantId)
    {
        return await _dbSet
            .Include(x => x.PaymentMethod)
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<TenantPaymentMethodEntity?> GetByIdWithPaymentMethodAsync(Guid id, Guid tenantId)
    {
        return await _dbSet
            .Include(x => x.PaymentMethod)
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
    }
}

