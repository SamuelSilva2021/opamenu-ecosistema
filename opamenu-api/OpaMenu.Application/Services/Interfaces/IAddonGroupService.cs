
using OpaMenu.Domain.DTOs;
using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Application.Services.Interfaces;

public interface IAddonGroupService
{
    // CRUD Operations
    Task<ResponseDTO<IEnumerable<AddonGroupResponseDto>>> GetAllAddonGroupsAsync();
    Task<ResponseDTO<AddonGroupResponseDto?>> GetAddonGroupByIdAsync(int id);
    Task<ResponseDTO<AddonGroupResponseDto?>> GetAddonGroupWithAddonsAsync(int id);
    Task<IEnumerable<AddonGroupEntity>> GetByProductIdAsync(int productId);
    Task<ResponseDTO<AddonGroupResponseDto>> CreateAddonGroupAsync(CreateAddonGroupRequestDto request);
    Task<ResponseDTO<AddonGroupResponseDto>> UpdateAddonGroupAsync(int id, UpdateAddonGroupRequestDto request);
    Task<ResponseDTO<bool>> DeleteAddonGroupAsync(int id);
    
    // Business Operations
    Task<ResponseDTO<AddonGroupResponseDto>> ToggleAddonGroupStatusAsync(int id);
    Task ReorderAddonGroupsAsync(Dictionary<int, int> groupOrders);
    Task<ProductAddonGroupEntity> AssignToProductAsync(int productId, int addonGroupId, AssignAddonGroupToProductRequestDto request);
    Task RemoveFromProductAsync(int productId, int addonGroupId);
    
    // Validation
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
    Task<bool> CanDeleteAddonGroupAsync(int id);
}
