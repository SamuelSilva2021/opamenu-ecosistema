using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface ICouponRepository : IRepository<CouponEntity>
{
    Task<IEnumerable<CouponEntity>> GetActiveCouponsAsync(Guid tenantId);
    Task<CouponEntity?> GetByCodeAsync(string code, Guid tenantId);
    Task<bool> IsCodeUniqueAsync(string code, Guid tenantId, Guid? excludeId = null);
}

