using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Domain.Interfaces;

public interface ICouponRepository : IRepository<CouponEntity>
{
    Task<IEnumerable<CouponEntity>> GetActiveCouponsAsync(Guid tenantId);
    Task<CouponEntity?> GetByCodeAsync(string code, Guid tenantId);
    Task<bool> IsCodeUniqueAsync(string code, Guid tenantId, Guid? excludeId = null);
}

