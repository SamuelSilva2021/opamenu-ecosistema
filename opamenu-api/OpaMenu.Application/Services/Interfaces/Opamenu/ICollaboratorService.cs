using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Collaborator;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

public interface ICollaboratorService
{
    Task<ResponseDTO<CollaboratorResponseDto>> CreateAsync(CreateCollaboratorRequestDto request);
    Task<ResponseDTO<CollaboratorResponseDto>> UpdateAsync(Guid id, UpdateCollaboratorRequestDto request);
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    Task<ResponseDTO<CollaboratorResponseDto>> GetByIdAsync(Guid id);
    Task<ResponseDTO<IEnumerable<CollaboratorResponseDto>>> GetAllAsync();
}
