using AutoMapper;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;

namespace OpaMenu.Application.Services;

public class AddonGroupService(
    IAddonGroupRepository addonGroupRepository,
    IProductRepository productRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
        ) : IAddonGroupService
{
    private readonly IAddonGroupRepository _addonGroupRepository = addonGroupRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDTO<IEnumerable<AddonGroupResponseDto>>> GetAllAddonGroupsAsync()
    {
        try
        {
            var groups = await _addonGroupRepository.GetByTenantIdAsync(_currentUserService.GetTenantGuid()!.Value);

            var dtos = _mapper.Map<IEnumerable<AddonGroupResponseDto>>(groups);
            return StaticResponseBuilder<IEnumerable<AddonGroupResponseDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<AddonGroupResponseDto>>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AddonGroupResponseDto?>> GetAddonGroupByIdAsync(Guid id)
    {
        try
        {
            var group = await _addonGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais nÃ£o encontrado");

            var dto = _mapper.Map<AddonGroupResponseDto>(group);
            return StaticResponseBuilder<AddonGroupResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonGroupResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AddonGroupResponseDto?>> GetAddonGroupWithAddonsAsync(Guid id)
    {
        try
        {
            var group = await _addonGroupRepository.GetWithAddonsAsync(id) ?? throw new ArgumentException("Grupo de adicionais nÃ£o encontrado");

            var dto = _mapper.Map<AddonGroupResponseDto>(group);
            return StaticResponseBuilder<AddonGroupResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonGroupResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<IEnumerable<AddonGroupEntity>> GetByProductIdAsync(Guid productId) =>
        await _addonGroupRepository.GetByProductIdAsync(productId);

    public async Task<ResponseDTO<AddonGroupResponseDto>> CreateAddonGroupAsync(CreateAddonGroupRequestDto request)
    {
        try
        {
            // Validar nome Ãºnico
            if (!await _addonGroupRepository.IsNameUniqueAsync(request.Name, null, _currentUserService.GetTenantGuid()!.Value))
                return StaticResponseBuilder<AddonGroupResponseDto>.BuildError("JÃ¡ existe um grupo de adicionais com este nome");

            var addonGroup = new AddonGroupEntity
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Type = request.Type,
                MinSelections = request.MinSelections,
                MaxSelections = request.MaxSelections,
                IsRequired = request.IsRequired,
                DisplayOrder = request.DisplayOrder > 0 ? request.DisplayOrder : await _addonGroupRepository.GetNextDisplayOrderAsync(_currentUserService.GetTenantGuid()!.Value),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TenantId = _currentUserService.GetTenantGuid()!.Value
            };

            var created = await _addonGroupRepository.AddAsync(addonGroup);
            var dto = _mapper.Map<AddonGroupResponseDto>(created);
            return StaticResponseBuilder<AddonGroupResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonGroupResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AddonGroupResponseDto>> UpdateAddonGroupAsync(Guid id, UpdateAddonGroupRequestDto request)
    {
        try
        {
            var addonGroup = await _addonGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (addonGroup == null)
                return StaticResponseBuilder<AddonGroupResponseDto>.BuildError("Grupo de adicionais nÃ£o encontrado");

            if (!await _addonGroupRepository.IsNameUniqueAsync(request.Name, id, _currentUserService.GetTenantGuid()!.Value))
                return StaticResponseBuilder<AddonGroupResponseDto>.BuildError("JÃ¡ existe um grupo de adicionais com este nome");

            addonGroup.Name = request.Name.Trim();
            addonGroup.Description = request.Description?.Trim();
            addonGroup.Type = request.Type;
            addonGroup.MinSelections = request.MinSelections;
            addonGroup.MaxSelections = request.MaxSelections;
            addonGroup.IsRequired = request.IsRequired;
            addonGroup.DisplayOrder = request.DisplayOrder;
            addonGroup.IsActive = request.IsActive;
            addonGroup.UpdatedAt = DateTime.UtcNow;

            await _addonGroupRepository.UpdateAsync(addonGroup);
            var dto = _mapper.Map<AddonGroupResponseDto>(addonGroup);
            return StaticResponseBuilder<AddonGroupResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonGroupResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> DeleteAddonGroupAsync(Guid id)
    {
        try
        {
            var addonGroup = await _addonGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (addonGroup == null)
                return StaticResponseBuilder<bool>.BuildError("Grupo de adicionais nÃ£o encontrado");

            if (!await CanDeleteAddonGroupAsync(id))
                return StaticResponseBuilder<bool>.BuildError("NÃ£o Ã© possÃ­vel excluir este grupo pois ele possui adicionais ou estÃ¡ associado a produtos");

            await _addonGroupRepository.DeleteVirtualAsync(id, _currentUserService.GetTenantGuid()!.Value);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<AddonGroupResponseDto>> ToggleAddonGroupStatusAsync(Guid id)
    {
        try
        {
            var addonGroup = await _addonGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais nÃ£o encontrado");

            addonGroup.IsActive = !addonGroup.IsActive;
            addonGroup.UpdatedAt = DateTime.UtcNow;

            await _addonGroupRepository.UpdateAsync(addonGroup);
            var dto = _mapper.Map<AddonGroupResponseDto>(addonGroup);
            return StaticResponseBuilder<AddonGroupResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AddonGroupResponseDto>.BuildErrorResponse(ex);
        }

    }

    public async Task ReorderAddonGroupsAsync(Dictionary<int, int> groupOrders) => 
        await _addonGroupRepository.UpdateDisplayOrdersAsync(groupOrders);

    public async Task<ProductAddonGroupEntity> AssignToProductAsync(Guid productId, Guid addonGroupId, AssignAddonGroupToProductRequestDto request)
    {
        // Verificar se produto existe
        var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Produto nÃ£o encontrado");

        // Verificar se grupo existe
        var addonGroup = await _addonGroupRepository.GetByIdAsync(addonGroupId, _currentUserService.GetTenantGuid()!.Value) ?? 
            throw new ArgumentException("Grupo de adicionais nÃ£o encontrado");

        var productAddonGroup = new ProductAddonGroupEntity
        {
            ProductId = productId,
            AddonGroupId = addonGroupId,
            DisplayOrder = request.DisplayOrder,
            IsRequired = request.IsRequired,
            MinSelectionsOverride = request.MinSelectionsOverride,
            MaxSelectionsOverride = request.MaxSelectionsOverride
        };
        return productAddonGroup;
    }

    public async Task RemoveFromProductAsync(Guid productId, Guid addonGroupId)
    {
        //TODO:
        // Implementar remoÃ§Ã£o da associaÃ§Ã£o produto-grupo
        // Esta implementaÃ§Ã£o precisaria de um repositÃ³rio especÃ­fico para ProductAddonGroup
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null) 
    {
        return await _addonGroupRepository.IsNameUniqueAsync(name, excludeId, _currentUserService.GetTenantGuid()!.Value);
    }

    public async Task<bool> CanDeleteAddonGroupAsync(Guid id) => 
        !await _addonGroupRepository.HasAddonsAsync(id);
}
