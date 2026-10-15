using Opamenu.Domain.DTOs;
using Opamenu.Application.DTOs;
using Opamenu.Domain.DTOs.AditionalGroup;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface IAditionalGroupService
{
    // CRUD Operations
    Task<ResponseDTO<IEnumerable<AditionalGroupResponseDto>>> GetAllAditionalGroupsAsync();
    Task<ResponseDTO<AditionalGroupResponseDto?>> GetAditionalGroupByIdAsync(Guid id);
    Task<ResponseDTO<AditionalGroupResponseDto?>> GetAditionalGroupWithAditionalsAsync(Guid id);
    Task<IEnumerable<AditionalGroupEntity>> GetByProductIdAsync(Guid productId);
    Task<ResponseDTO<AditionalGroupResponseDto>> CreateAditionalGroupAsync(CreateAditionalGroupRequestDto request);
    Task<ResponseDTO<AditionalGroupResponseDto>> UpdateAditionalGroupAsync(Guid id, UpdateAditionalGroupRequestDto request);
    Task<ResponseDTO<bool>> DeleteAditionalGroupAsync(Guid id);
    
    // Business Operations
    Task<ResponseDTO<AditionalGroupResponseDto>> ToggleAditionalGroupStatusAsync(Guid id);
    Task ReorderAditionalGroupsAsync(Dictionary<int, int> groupOrders);
    Task<ProductAditionalGroupEntity> AssignToProductAsync(Guid productId, Guid aditionalGroupId, AssignAditionalGroupToProductRequestDto request);
    Task RemoveFromProductAsync(Guid productId, Guid aditionalGroupId);
    
    // Validation
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    Task<bool> CanDeleteAditionalGroupAsync(Guid id);
}
