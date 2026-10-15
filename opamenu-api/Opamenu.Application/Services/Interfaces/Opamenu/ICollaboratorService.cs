using Opamenu.Commons.Api.DTOs;
using Opamenu.Domain.DTOs.Collaborator;

namespace Opamenu.Application.Services.Interfaces.Opamenu;

public interface ICollaboratorService
{
    Task<ResponseDTO<CollaboratorResponseDto>> CreateAsync(CreateCollaboratorRequestDto request);
    Task<ResponseDTO<CollaboratorResponseDto>> UpdateAsync(Guid id, UpdateCollaboratorRequestDto request);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<CollaboratorResponseDto>> GetByIdAsync(Guid id);
    Task<ResponseDTO<IEnumerable<CollaboratorResponseDto>>> GetAllAsync();
}
