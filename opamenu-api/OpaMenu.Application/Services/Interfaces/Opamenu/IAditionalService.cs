using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Aditionals;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface IAditionalService
{
    // CRUD Operations
    Task<ResponseDTO<IEnumerable<AditionalResponseDto>>> GetAllAditionalsAsync();
    Task<IEnumerable<AditionalResponseDto>> GetByAditionalGroupIdAsync(Guid aditionalGroupId);
    Task<ResponseDTO<AditionalResponseDto?>> GetAditionalByIdAsync(Guid id);
    Task<ResponseDTO<AditionalResponseDto?>> CreateAditionalAsync(CreateAditionalRequestDto request);
    Task<ResponseDTO<AditionalResponseDto>> UpdateAditionalAsync(Guid id, UpdateAditionalRequestDto request);
    Task<ResponseDTO<bool>> DeleteAditionalAsync(Guid id);
    // Business Operations
    Task<ResponseDTO<AditionalResponseDto>> ToggleAditionalStatusAsync(Guid id);

    // Validation
    Task<bool> IsNameUniqueInGroupAsync(string name, Guid aditionalGroupId, Guid? excludeId = null);
    Task<bool> CanDeleteAditionalAsync(Guid id);
}
