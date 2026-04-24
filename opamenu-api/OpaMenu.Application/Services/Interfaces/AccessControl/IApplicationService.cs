using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IApplicationService
{
    Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(int page, int limit, string? search);
    Task<ApplicationDto?> GetByIdAsync(Guid id);
    Task<ApplicationDto?> CreateAsync(CreateApplicationRequestDto request);
    Task<(ApplicationDto? Application, bool NotFound)> UpdateAsync(Guid id, UpdateApplicationRequestDto request);
    Task DeleteAsync(Guid id);
}

