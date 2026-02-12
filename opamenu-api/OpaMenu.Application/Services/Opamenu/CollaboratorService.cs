using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Collaborator;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Interfaces;

namespace OpaMenu.Application.Services.Opamenu;

public class CollaboratorService(
    ICollaboratorRepository repository,
    ITenantContext tenantContext,
    ICurrentUserService currentUserService,
    ILogger<CollaboratorService> logger) : ICollaboratorService
{
    private readonly ICollaboratorRepository _repository = repository;
    private readonly ITenantContext _tenantContext = tenantContext;
    private readonly ILogger<CollaboratorService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;


    public async Task<ResponseDTO<CollaboratorResponseDto>> CreateAsync(CreateCollaboratorRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (tenantId == null || tenantId == Guid.Empty)
            return StaticResponseBuilder<CollaboratorResponseDto>.BuildError("Tenant não identificado");

        if (!string.IsNullOrEmpty(request.Phone))
            request.Phone = System.Text.RegularExpressions.Regex.Replace(request.Phone, @"\D", "");

        var entity = new CollaboratorEntity
        {
            Name = request.Name,
            Phone = request.Phone,
            Type = request.Type,
            Role = request.Role,
            UserAccountId = request.UserAccountId,
            TenantId = tenantId.Value,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);

        return StaticResponseBuilder<CollaboratorResponseDto>.BuildOk(MapToDto(entity));
    }

    public async Task<ResponseDTO<CollaboratorResponseDto>> UpdateAsync(Guid id, UpdateCollaboratorRequestDto request)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (tenantId == null || tenantId == Guid.Empty)
            return StaticResponseBuilder<CollaboratorResponseDto>.BuildError("Tenant não identificado");

        var entity = await _repository.GetByIdAsync(id, tenantId.Value);
        if (entity == null)
            return StaticResponseBuilder<CollaboratorResponseDto>.BuildError("Colaborador não encontrado");

        if (request.Name != null) entity.Name = request.Name;
        if (request.Phone != null)
        {
            entity.Phone = System.Text.RegularExpressions.Regex.Replace(request.Phone, @"\D", "");
        }
        if (request.Type.HasValue) entity.Type = request.Type.Value;
        if (request.Role != null) entity.Role = request.Role;
        if (request.Active.HasValue) entity.Active = request.Active.Value;
        if (request.UserAccountId.HasValue) entity.UserAccountId = request.UserAccountId;

        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);

        return StaticResponseBuilder<CollaboratorResponseDto>.BuildOk(MapToDto(entity));
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (tenantId == null || tenantId == Guid.Empty)
            return StaticResponseBuilder<bool>.BuildError("Tenant não identificado");

        var entity = await _repository.GetByIdAsync(id, tenantId.Value);
        if (entity == null)
            return StaticResponseBuilder<bool>.BuildError("Colaborador não encontrado");

        // Soft delete
        entity.Active = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        
        return StaticResponseBuilder<bool>.BuildOk(true);
    }

    public async Task<ResponseDTO<CollaboratorResponseDto>> GetByIdAsync(Guid id)
    {
        var tenantId = _currentUserService.GetTenantGuid();
        if (tenantId == null || tenantId == Guid.Empty)
            return StaticResponseBuilder<CollaboratorResponseDto>.BuildError("Tenant não identificado");

        var entity = await _repository.GetByIdAsync(id, tenantId.Value);
        if (entity == null)
            return StaticResponseBuilder<CollaboratorResponseDto>.BuildError("Colaborador não encontrado");

        return StaticResponseBuilder<CollaboratorResponseDto>.BuildOk(MapToDto(entity));
    }

    public async Task<ResponseDTO<IEnumerable<CollaboratorResponseDto>>> GetAllAsync()
    {
        var tenantId = _currentUserService.GetTenantGuid();

        if (tenantId == null || tenantId == Guid.Empty)
            return StaticResponseBuilder<IEnumerable<CollaboratorResponseDto>>.BuildError("Tenant não identificado");

        var entities = await _repository.GetActiveByTenantIdAsync(tenantId.Value);
        
        var dtos = entities.Select(MapToDto);
        return StaticResponseBuilder<IEnumerable<CollaboratorResponseDto>>.BuildOk(dtos);
    }

    private static CollaboratorResponseDto MapToDto(CollaboratorEntity entity)
    {
        return new CollaboratorResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Phone = entity.Phone,
            Type = entity.Type,
            Role = entity.Role,
            Active = entity.Active,
            UserAccountId = entity.UserAccountId,
            TenantId = entity.TenantId,
            CreatedAt = entity.CreatedAt
        };
    }
}
