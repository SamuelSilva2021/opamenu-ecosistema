using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class CashRegisterRepository(OpamenuDbContext context) : OpamenuRepository<CashShiftEntity>(context), ICashRegisterRepository
{
    public async Task<CashShiftEntity?> GetActiveShiftAsync(Guid userId, Guid tenantId)
    {
        return await _dbSet
            .Include(s => s.Movements)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TenantId == tenantId && s.Status == ECashShiftStatus.Open);
    }

    public async Task<IEnumerable<CashShiftEntity>> GetShiftHistoryAsync(Guid tenantId, int count)
    {
        return await _dbSet
            .Where(s => s.TenantId == tenantId)
            .OrderByDescending(s => s.OpenedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<CashShiftEntity>> GetShiftsByPeriodAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(s => s.Movements)
            .Where(s => s.TenantId == tenantId && s.OpenedAt >= startDate && s.OpenedAt <= endDate)
            .OrderByDescending(s => s.OpenedAt)
            .ToListAsync();
    }

    public async Task AddMovementAsync(CashMovementEntity movement)
    {
        movement.CreatedAt = DateTime.UtcNow;
        movement.UpdatedAt = DateTime.UtcNow;
        await _context.Set<CashMovementEntity>().AddAsync(movement);
        await _context.SaveChangesAsync();
    }

    public async Task<CashMovementEntity?> GetMovementByOrderAsync(Guid tenantId, Guid shiftId, Guid orderId, ECashMovementType type)
    {
        return await _context.Set<CashMovementEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(m =>
                m.TenantId == tenantId &&
                m.ShiftId == shiftId &&
                m.OrderId == orderId &&
                m.Type == type);
    }
}
