using Opamenu.Domain.DTOs.AccessControl;

namespace Opamenu.Application.Services.Interfaces.AccessControl;

public interface IPermissionOperationService
{
    Task<(bool Success, bool NotFound, bool BadRequest)> BulkAsync(PermissionOperationBulkRequestDto request);
}

