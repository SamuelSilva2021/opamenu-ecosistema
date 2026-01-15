using OpaMenu.Application.DTOs;
using OpaMenu.Application.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Services.Interfaces;

public interface IAddonService
{
    // CRUD Operations
    Task<ResponseDTO<IEnumerable<AddonResponseDto>>> GetAllAddonsAsync();
    Task<IEnumerable<AddonResponseDto>> GetByAddonGroupIdAsync(int addonGroupId);
    Task<ResponseDTO<AddonResponseDto?>> GetAddonByIdAsync(int id);
    Task<ResponseDTO<AddonResponseDto?>> CreateAddonAsync(CreateAddonRequestDto request);
    Task<ResponseDTO<AddonResponseDto>> UpdateAddonAsync(int id, UpdateAddonRequestDto request);
    Task<ResponseDTO<bool>> DeleteAddonAsync(int id);
    // Business Operations
    Task<ResponseDTO<AddonResponseDto>> ToggleAddonStatusAsync(int id);

    // Validation
    Task<bool> IsNameUniqueInGroupAsync(string name, int addonGroupId, int? excludeId = null);
    Task<bool> CanDeleteAddonAsync(int id);
}
