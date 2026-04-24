using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.AccessControl;

namespace OpaMenu.Application.Services.Interfaces.AccessControl;

public interface IUserService
{
    Task<PagedResultDto<UserAccountDto>> GetUsersAsync(int page, int limit, string? search);
    Task<List<UserAccountDto>> GetActiveUsersAsync();
    Task<UserAccountDto?> GetByIdAsync(Guid id);
    Task<UserAccountDto?> CreateAsync(CreateUserAccountRequestDto request);
    Task<(UserAccountDto? User, bool NotFound)> UpdateAsync(Guid id, UpdateUserAccountRequestDto request);
    Task DeleteAsync(Guid id);

    Task<ResponseDTO<PagedResultDto<UserAccountDto>>> GetEmployeesPainelAsync(Guid tenantId, int page, int limit, string? search);
    Task<ResponseDTO<UserAccountDto>> GetEmployeePainelByIdAsync(Guid tenantId, Guid id);
    Task<ResponseDTO<UserAccountDto>> CreateEmployeePainelAsync(Guid tenantId, CreateUserAccountRequestDto request);
    Task<ResponseDTO<UserAccountDto>> UpdateEmployeePainelAsync(Guid tenantId, Guid id, UpdateUserAccountRequestDto request);
    Task<ResponseDTO<UserAccountDto>> ToggleEmployeeStatusPainelAsync(Guid tenantId, Guid id);
    Task<ResponseDTO<bool>> DeleteEmployeePainelAsync(Guid tenantId, Guid id);

    Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);

    Task<List<AccessGroupDto>> GetUserAccessGroupsAsync(Guid userId);
    Task<(bool Success, bool NotFound, bool BadRequest)> AssignUserAccessGroupsAsync(Guid userId, AssignUserAccessGroupsRequestDto request);
    Task<bool> RevokeUserAccessGroupAsync(Guid userId, Guid groupId);
}

