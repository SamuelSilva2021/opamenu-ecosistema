using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Aditionals;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class AditionalService(
    IAditionalRepository aditionalRepository, 
    IAditionalGroupRepository aditionalGroupRepository, 
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IAditionalService
{
    private readonly IAditionalRepository _aditionalRepository = aditionalRepository;
    private readonly IAditionalGroupRepository _aditionalGroupRepository = aditionalGroupRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDTO<IEnumerable<AditionalResponseDto>>> GetAllAditionalsAsync()
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                throw new UnauthorizedAccessException("Usuário não autenticado ou sem tenant.");

            var aditionalsEntity = await _aditionalRepository.GetAllAditionalsAsync(tenantId.Value);
            var aditionals = _mapper.Map<IEnumerable<AditionalResponseDto>>(aditionalsEntity);

            return StaticResponseBuilder<IEnumerable<AditionalResponseDto>>.BuildOk(aditionals);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<AditionalResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<IEnumerable<AditionalResponseDto>> GetByAditionalGroupIdAsync(Guid aditionalGroupId)
    {
        var aditionals = await _aditionalRepository.GetByAditionalGroupIdAsync(aditionalGroupId);

        return _mapper.Map<IEnumerable<AditionalResponseDto>>(aditionals);
    }

    public async Task<ResponseDTO<AditionalResponseDto?>> GetAditionalByIdAsync(Guid id)
    {
        try
        {
            var aditionalEntity = await _aditionalRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            var aditional = _mapper.Map<AditionalResponseDto?>(aditionalEntity);

            return StaticResponseBuilder<AditionalResponseDto?>.BuildOk(aditional);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalResponseDto?>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<AditionalResponseDto?>> CreateAditionalAsync(CreateAditionalRequestDto request)
    {
        try
        {
            var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(request.AditionalGroupId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais não encontrado");
            if (!await _aditionalRepository.IsNameUniqueInGroupAsync(request.Name, request.AditionalGroupId))
                throw new ArgumentException("Já existe um adicional com este nome neste grupo");

            var aditionalEntity = _mapper.Map<AditionalEntity>(request);
            aditionalEntity.TenantId = _currentUserService.GetTenantGuid()!.Value;

            var aditionalCreated = await _aditionalRepository.AddAsync(aditionalEntity);
            var aditional = _mapper.Map<AditionalResponseDto>(aditionalCreated);

            return StaticResponseBuilder<AditionalResponseDto?>.BuildOk(aditional);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AditionalResponseDto>> UpdateAditionalAsync(Guid id, UpdateAditionalRequestDto request)
    {
        try
        {
            var aditionalEntity = await _aditionalRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Adicional não encontrado");

            if (aditionalEntity.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Adicional não pertence a esse cliente");

            var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(request.AditionalGroupId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais não encontrado");

            if (aditionalGroup.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Grupo de adicionais não pertence a esse cliente");

            // Validar nome único no NOVO grupo (se o grupo mudou) ou no grupo atual
            if (!await _aditionalRepository.IsNameUniqueInGroupAsync(request.Name, request.AditionalGroupId, id))
                throw new ArgumentException("Já existe um adicional com este nome neste grupo");

            aditionalEntity = _mapper.Map(request, aditionalEntity);

            await _aditionalRepository.UpdateAsync(aditionalEntity);

            var aditional = _mapper.Map<AditionalResponseDto>(aditionalEntity);
            return StaticResponseBuilder<AditionalResponseDto>.BuildOk(aditional);

        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> DeleteAditionalAsync(Guid id)
    {
        try
        {
            var aditionalEntity = await _aditionalRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (aditionalEntity == null)
                throw new ArgumentException("Adicional não encontrado");

            if (aditionalEntity.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Adicional não pertence a esse cliente");

            if (!await CanDeleteAditionalAsync(id))
                throw new InvalidOperationException("Não é possível excluir este adicional pois ele está sendo usado em pedidos");

            await _aditionalRepository.DeleteAsync(aditionalEntity);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AditionalResponseDto>> ToggleAditionalStatusAsync(Guid id)
    {
        try
        {
            var aditionalEntity = await _aditionalRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (aditionalEntity == null)
                throw new ArgumentException("Adicional não encontrado");

            if (aditionalEntity.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Adicional não pertence a esse cliente");

            aditionalEntity.IsActive = !aditionalEntity.IsActive;
            aditionalEntity.UpdatedAt = DateTime.UtcNow;

            await _aditionalRepository.UpdateAsync(aditionalEntity);

            var aditional = _mapper.Map<AditionalResponseDto>(aditionalEntity);

            return StaticResponseBuilder<AditionalResponseDto>.BuildOk(aditional);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalResponseDto>.BuildErrorResponse(ex);
        }

    }
    public async Task<bool> IsNameUniqueInGroupAsync(string name, Guid aditionalGroupId, Guid? excludeId = null) =>
        await _aditionalRepository.IsNameUniqueInGroupAsync(name, aditionalGroupId, excludeId);

    public Task<bool> CanDeleteAditionalAsync(Guid id)
    {
        //TODO:
        // Verificar se o adicional está sendo usado em algum pedido
        // Esta lógica pode ser implementada conforme necessário
        return Task.FromResult(true);
    }
}
