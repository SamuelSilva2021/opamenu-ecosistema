using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts
{
    public interface IUserAccountPainelService
    {
        Task<ResponseDTO<PagedResponseDTO<UserAccountDTO>>> GetAllEmployeePagedAsync(int page, int limit, string? search = null);
        Task<ResponseDTO<UserAccountDTO>> GetEmployeeByIdAsync(Guid id);
        Task<ResponseDTO<UserAccountDTO>> CreateEmployeeAsync(UserAccountCreateDTO dto);
        Task<ResponseDTO<UserAccountDTO>> UpdateEmployeeAsync(Guid id, UserAccountUpdateDto dto);
        Task<ResponseDTO<UserAccountDTO>> ToggleEmployeeStatusAsync(Guid id);
        Task<ResponseDTO<bool>> DeleteEmployeeAsync(Guid id);
    }
}
