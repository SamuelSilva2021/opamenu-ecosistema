
using OpaMenu.Domain.DTOs;
using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface IAddonGroupService
{
    // CRUD Operations
    Task<ResponseDTO<IEnumerable<AddonGroupResponseDto>>> GetAllAddonGroupsAsync();
    Task<ResponseDTO<AddonGroupResponseDto?>> GetAddonGroupByIdAsync(Guid id);
    Task<ResponseDTO<AddonGroupResponseDto?>> GetAddonGroupWithAddonsAsync(Guid id);
    Task<IEnumerable<AddonGroupEntity>> GetByProductIdAsync(Guid productId);
    Task<ResponseDTO<AddonGroupResponseDto>> CreateAddonGroupAsync(CreateAddonGroupRequestDto request);
    Task<ResponseDTO<AddonGroupResponseDto>> UpdateAddonGroupAsync(Guid id, UpdateAddonGroupRequestDto request);
    Task<ResponseDTO<bool>> DeleteAddonGroupAsync(Guid id);
    
    // Business Operations
    Task<ResponseDTO<AddonGroupResponseDto>> ToggleAddonGroupStatusAsync(Guid id);
    Task ReorderAddonGroupsAsync(Dictionary<int, int> groupOrders);
    Task<ProductAddonGroupEntity> AssignToProductAsync(Guid productId, Guid addonGroupId, AssignAddonGroupToProductRequestDto request);
    Task RemoveFromProductAsync(Guid productId, Guid addonGroupId);
    
    // Validation
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    Task<bool> CanDeleteAddonGroupAsync(Guid id);
}
