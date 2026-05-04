using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Repositories;

public class DeliveryAreaRepository(OpamenuDbContext context) 
    : OpamenuRepository<DeliveryAreaEntity>(context), IDeliveryAreaRepository
{
    public async Task<DeliveryAreaEntity?> GetByLocationAsync(Guid tenantId, string city, string? neighborhood)
    {
        var query = _dbSet.Where(x => x.TenantId == tenantId && x.IsActive && x.City.ToLower() == city.ToLower());

        if (!string.IsNullOrEmpty(neighborhood))
        {
            // Tenta encontrar o bairro exato primeiro
            var neighborhoodRule = await query.FirstOrDefaultAsync(x => x.Neighborhood != null && x.Neighborhood.ToLower() == neighborhood.ToLower());
            if (neighborhoodRule != null) return neighborhoodRule;
        }

        // Se não encontrar o bairro (ou não foi informado), pega a regra da cidade (onde neighborhood é null ou vazio)
        return await query.FirstOrDefaultAsync(x => string.IsNullOrEmpty(x.Neighborhood));
    }

    public async Task<IEnumerable<DeliveryAreaEntity>> GetAllByTenantAsync(Guid tenantId)
    {
        return await _dbSet
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.City)
            .ThenBy(x => x.Neighborhood)
            .ToListAsync();
    }
}
