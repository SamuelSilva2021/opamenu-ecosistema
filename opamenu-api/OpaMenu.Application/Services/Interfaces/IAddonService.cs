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
    Task<IEnumerable<AddonResponseDto>> GetByAddonGroupIdAsync(Guid addonGroupId);
    Task<ResponseDTO<AddonResponseDto?>> GetAddonByIdAsync(Guid id);
    Task<ResponseDTO<AddonResponseDto?>> CreateAddonAsync(CreateAddonRequestDto request);
    Task<ResponseDTO<AddonResponseDto>> UpdateAddonAsync(Guid id, UpdateAddonRequestDto request);
    Task<ResponseDTO<bool>> DeleteAddonAsync(Guid id);
    // Business Operations
    Task<ResponseDTO<AddonResponseDto>> ToggleAddonStatusAsync(Guid id);

    // Validation
    Task<bool> IsNameUniqueInGroupAsync(string name, Guid addonGroupId, Guid? excludeId = null);
    Task<bool> CanDeleteAddonAsync(Guid id);
}
