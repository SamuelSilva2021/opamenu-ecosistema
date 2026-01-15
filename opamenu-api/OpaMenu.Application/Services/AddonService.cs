using AutoMapper;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;

namespace OpaMenu.Application.Services;

public class AddonService(
    IAddonRepository addonRepository, 
    IAddonGroupRepository addonGroupRepository, 
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IAddonService
{
    private readonly IAddonRepository _addonRepository = addonRepository;
    private readonly IAddonGroupRepository _addonGroupRepository = addonGroupRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDTO<IEnumerable<AddonResponseDto>>> GetAllAddonsAsync()
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                throw new UnauthorizedAccessException("UsuÃ¡rio nÃ£o autenticado ou sem tenant.");

            var addonsEntity = await _addonRepository.GetAllAddonsAsync(tenantId.Value);
            var addons = _mapper.Map<IEnumerable<AddonResponseDto>>(addonsEntity);

            return StaticResponseBuilder<IEnumerable<AddonResponseDto>>.BuildOk(addons);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<AddonResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<IEnumerable<AddonResponseDto>> GetByAddonGroupIdAsync(int addonGroupId)
    {
        var addons = await _addonRepository.GetByAddonGroupIdAsync(addonGroupId);

        return _mapper.Map<IEnumerable<AddonResponseDto>>(addons);
    }

    public async Task<ResponseDTO<AddonResponseDto?>> GetAddonByIdAsync(int id)
    {
        try
        {
            var addonEntity = await _addonRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            var addon = _mapper.Map<AddonResponseDto?>(addonEntity);

            return StaticResponseBuilder<AddonResponseDto?>.BuildOk(addon);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonResponseDto?>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<AddonResponseDto?>> CreateAddonAsync(CreateAddonRequestDto request)
    {
        try
        {
            var addonGroup = await _addonGroupRepository.GetByIdAsync(request.AddonGroupId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais nÃ£o encontrado");
            if (!await _addonRepository.IsNameUniqueInGroupAsync(request.Name, request.AddonGroupId))
                throw new ArgumentException("JÃ¡ existe um adicional com este nome neste grupo");

            var addonEntity = _mapper.Map<AddonEntity>(request);
            addonEntity.TenantId = _currentUserService.GetTenantGuid()!.Value;

            var addonCreated = await _addonRepository.AddAsync(addonEntity);
            var addon = _mapper.Map<AddonResponseDto>(addonCreated);

            return StaticResponseBuilder<AddonResponseDto?>.BuildOk(addon);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AddonResponseDto>> UpdateAddonAsync(int id, UpdateAddonRequestDto request)
    {
        try
        {
            var addonEntity = await _addonRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Adicional nÃ£o encontrado");

            if (addonEntity.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Adicional nÃ£o pertence a esse cliente");

            var addonGroup = await _addonGroupRepository.GetByIdAsync(request.AddonGroupId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais nÃ£o encontrado");

            if (addonGroup.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Grupo de adicionais nÃ£o pertence a esse cliente");

            // Validar nome Ãºnico no NOVO grupo (se o grupo mudou) ou no grupo atual
            if (!await _addonRepository.IsNameUniqueInGroupAsync(request.Name, request.AddonGroupId, id))
                throw new ArgumentException("JÃ¡ existe um adicional com este nome neste grupo");

            addonEntity = _mapper.Map(request, addonEntity);

            await _addonRepository.UpdateAsync(addonEntity);

            var addon = _mapper.Map<AddonResponseDto>(addonEntity);
            return StaticResponseBuilder<AddonResponseDto>.BuildOk(addon);

        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> DeleteAddonAsync(int id)
    {
        try
        {
            var addonEntity = await _addonRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (addonEntity == null)
                throw new ArgumentException("Adicional nÃ£o encontrado");

            if (addonEntity.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Adicional nÃ£o pertence a esse cliente");

            if (!await CanDeleteAddonAsync(id))
                throw new InvalidOperationException("NÃ£o Ã© possÃ­vel excluir este adicional pois ele estÃ¡ sendo usado em pedidos");

            await _addonRepository.DeleteAsync(addonEntity);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AddonResponseDto>> ToggleAddonStatusAsync(int id)
    {
        try
        {
            var addonEntity = await _addonRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (addonEntity == null)
                throw new ArgumentException("Adicional nÃ£o encontrado");

            if (addonEntity.TenantId != _currentUserService.GetTenantGuid()!.Value)
                throw new ArgumentException("Adicional nÃ£o pertence a esse cliente");

            addonEntity.IsActive = !addonEntity.IsActive;
            addonEntity.UpdatedAt = DateTime.UtcNow;

            await _addonRepository.UpdateAsync(addonEntity);

            var addon = _mapper.Map<AddonResponseDto>(addonEntity);

            return StaticResponseBuilder<AddonResponseDto>.BuildOk(addon);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonResponseDto>.BuildErrorResponse(ex);
        }

    }
    public async Task<bool> IsNameUniqueInGroupAsync(string name, int addonGroupId, int? excludeId = null) =>
        await _addonRepository.IsNameUniqueInGroupAsync(name, addonGroupId, excludeId);

    public Task<bool> CanDeleteAddonAsync(int id)
    {
        //TODO:
        // Verificar se o adicional estÃ¡ sendo usado em algum pedido
        // Esta lÃ³gica pode ser implementada conforme necessÃ¡rio
        return Task.FromResult(true);
    }
}
