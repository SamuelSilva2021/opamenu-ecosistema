using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Auth;

namespace Opamenu.Application.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<ResponseDTO<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ResponseDTO<LoginResponseDto>> RefreshAsync(string refreshToken);
    Task<ResponseDTO<bool>> LogoutAsync(string refreshToken);
    Task<ResponseDTO<UserInfoDto>> GetMeAsync(Guid userId, string? tenantSlug);
}

