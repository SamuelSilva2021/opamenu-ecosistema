using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Auth;

namespace OpaMenu.Application.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<ResponseDTO<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ResponseDTO<LoginResponseDto>> RefreshAsync(string refreshToken);
    Task<ResponseDTO<bool>> LogoutAsync(string refreshToken);
    Task<ResponseDTO<UserInfoDto>> GetMeAsync(Guid userId, string? tenantSlug);
}

