using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class PermissionOperationService(IRolePermissionRepository rolePermissionRepository) : IPermissionOperationService
{
    private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;

    public async Task<(bool Success, bool NotFound, bool BadRequest)> BulkAsync(PermissionOperationBulkRequestDto request)
    {
        if (request.PermissionId == Guid.Empty)
        {
            return (false, false, true);
        }

        var rp = await _rolePermissionRepository.GetByIdTrackedAsync(request.PermissionId);
        if (rp == null)
        {
            return (false, true, false);
        }

        rp.Actions = NormalizeOperationIds(request.OperationIds);
        rp.UpdatedAt = DateTime.UtcNow;
        await _rolePermissionRepository.SaveChangesAsync();
        return (true, false, false);
    }

    private static List<string> NormalizeOperationIds(List<string> operationIds)
    {
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "CREATE", "READ", "UPDATE", "DELETE", "SELECT" };

        if (operationIds.Count == 0)
        {
            return [];
        }

        return operationIds
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToUpperInvariant())
            .Where(allowed.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

