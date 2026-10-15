using Opamenu.Infrastructure.Shared.Entities.AccessControl;

namespace Opamenu.Domain.Interfaces;

public interface IApplicationRepository
{
    Task<(IReadOnlyList<ApplicationEntity> Items, int Total)> GetPagedAsync(int page, int limit, string? search);
    Task<ApplicationEntity?> GetByIdAsync(Guid id);
    Task<ApplicationEntity?> GetByIdTrackedAsync(Guid id);
    Task AddAsync(ApplicationEntity entity);
    Task DeleteAsync(ApplicationEntity entity);
    Task SaveChangesAsync();
}

