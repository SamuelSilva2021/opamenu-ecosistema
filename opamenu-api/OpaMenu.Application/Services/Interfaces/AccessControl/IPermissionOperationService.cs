using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IPermissionOperationService
{
    Task<(bool Success, bool NotFound, bool BadRequest)> BulkAsync(PermissionOperationBulkRequestDto request);
}

