using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Domain.Interfaces;

public interface ICashRegisterRepository : IRepository<CashShiftEntity>
{
    Task<CashShiftEntity?> GetActiveShiftAsync(Guid userId, Guid tenantId);
    Task<IEnumerable<CashShiftEntity>> GetShiftHistoryAsync(Guid tenantId, int count);
    Task<IEnumerable<CashShiftEntity>> GetShiftsByPeriodAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    Task AddMovementAsync(CashMovementEntity movement);
    Task<CashMovementEntity?> GetMovementByOrderAsync(Guid tenantId, Guid shiftId, Guid orderId, ECashMovementType type);
}
