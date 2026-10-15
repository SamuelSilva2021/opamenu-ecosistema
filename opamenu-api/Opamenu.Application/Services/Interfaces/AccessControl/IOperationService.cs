using Opamenu.Domain.DTOs.AccessControl;

namespace Opamenu.Application.Services.Interfaces.AccessControl;

public interface IOperationService
{
    Task<PagedResultDto<OperationDto>> GetAllAsync(int page, int limit, string? search);
    Task<OperationDto?> GetByIdAsync(string id);
}

