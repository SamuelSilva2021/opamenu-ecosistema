using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AditionalGroup;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class AditionalGroupService(
    IAditionalGroupRepository aditionalGroupRepository,
    IProductRepository productRepository,
    ICurrentUserService currentUserService,
    IMapper mapper
        ) : IAditionalGroupService
{
    private readonly IAditionalGroupRepository _aditionalGroupRepository = aditionalGroupRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDTO<IEnumerable<AditionalGroupResponseDto>>> GetAllAditionalGroupsAsync()
    {
        try
        {
            var groups = await _aditionalGroupRepository.GetByTenantIdAsync(_currentUserService.GetTenantGuid()!.Value);

            var dtos = _mapper.Map<IEnumerable<AditionalGroupResponseDto>>(groups);
            return StaticResponseBuilder<IEnumerable<AditionalGroupResponseDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<AditionalGroupResponseDto>>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AditionalGroupResponseDto?>> GetAditionalGroupByIdAsync(Guid id)
    {
        try
        {
            var group = await _aditionalGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais não encontrado");

            var dto = _mapper.Map<AditionalGroupResponseDto>(group);
            return StaticResponseBuilder<AditionalGroupResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalGroupResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AditionalGroupResponseDto?>> GetAditionalGroupWithAditionalsAsync(Guid id)
    {
        try
        {
            var group = await _aditionalGroupRepository.GetWithAditionalsAsync(id) ?? throw new ArgumentException("Grupo de adicionais não encontrado");

            var dto = _mapper.Map<AditionalGroupResponseDto>(group);
            return StaticResponseBuilder<AditionalGroupResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalGroupResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<IEnumerable<AditionalGroupEntity>> GetByProductIdAsync(Guid productId) =>
        await _aditionalGroupRepository.GetByProductIdAsync(productId);

    public async Task<ResponseDTO<AditionalGroupResponseDto>> CreateAditionalGroupAsync(CreateAditionalGroupRequestDto request)
    {
        try
        {
            // Validar nome único
            if (!await _aditionalGroupRepository.IsNameUniqueAsync(request.Name, null, _currentUserService.GetTenantGuid()!.Value))
                return StaticResponseBuilder<AditionalGroupResponseDto>.BuildError("Já existe um grupo de adicionais com este nome");

            var aditionalGroup = new AditionalGroupEntity
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Type = request.Type,
                MinSelections = request.MinSelections,
                MaxSelections = request.MaxSelections,
                IsRequired = request.IsRequired,
                DisplayOrder = request.DisplayOrder > 0 ? request.DisplayOrder : await _aditionalGroupRepository.GetNextDisplayOrderAsync(_currentUserService.GetTenantGuid()!.Value),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TenantId = _currentUserService.GetTenantGuid()!.Value
            };

            var created = await _aditionalGroupRepository.AddAsync(aditionalGroup);
            var dto = _mapper.Map<AditionalGroupResponseDto>(created);
            return StaticResponseBuilder<AditionalGroupResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalGroupResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<AditionalGroupResponseDto>> UpdateAditionalGroupAsync(Guid id, UpdateAditionalGroupRequestDto request)
    {
        try
        {
            var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (aditionalGroup == null)
                return StaticResponseBuilder<AditionalGroupResponseDto>.BuildError("Grupo de adicionais não encontrado");

            if (!await _aditionalGroupRepository.IsNameUniqueAsync(request.Name, id, _currentUserService.GetTenantGuid()!.Value))
                return StaticResponseBuilder<AditionalGroupResponseDto>.BuildError("Já existe um grupo de adicionais com este nome");

            aditionalGroup.Name = request.Name.Trim();
            aditionalGroup.Description = request.Description?.Trim();
            aditionalGroup.Type = request.Type;
            aditionalGroup.MinSelections = request.MinSelections;
            aditionalGroup.MaxSelections = request.MaxSelections;
            aditionalGroup.IsRequired = request.IsRequired;
            aditionalGroup.DisplayOrder = request.DisplayOrder;
            aditionalGroup.IsActive = request.IsActive;
            aditionalGroup.UpdatedAt = DateTime.UtcNow;

            await _aditionalGroupRepository.UpdateAsync(aditionalGroup);
            var dto = _mapper.Map<AditionalGroupResponseDto>(aditionalGroup);
            return StaticResponseBuilder<AditionalGroupResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalGroupResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> DeleteAditionalGroupAsync(Guid id)
    {
        try
        {
            var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (aditionalGroup == null)
                return StaticResponseBuilder<bool>.BuildError("Grupo de adicionais não encontrado");

            if (!await CanDeleteAditionalGroupAsync(id))
                return StaticResponseBuilder<bool>.BuildError("Não é possível excluir este grupo pois ele possui adicionais ou está associado a produtos");

            await _aditionalGroupRepository.DeleteVirtualAsync(id, _currentUserService.GetTenantGuid()!.Value);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<AditionalGroupResponseDto>> ToggleAditionalGroupStatusAsync(Guid id)
    {
        try
        {
            var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Grupo de adicionais não encontrado");

            aditionalGroup.IsActive = !aditionalGroup.IsActive;
            aditionalGroup.UpdatedAt = DateTime.UtcNow;

            await _aditionalGroupRepository.UpdateAsync(aditionalGroup);
            var dto = _mapper.Map<AditionalGroupResponseDto>(aditionalGroup);
            return StaticResponseBuilder<AditionalGroupResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<AditionalGroupResponseDto>.BuildErrorResponse(ex);
        }

    }

    public async Task ReorderAditionalGroupsAsync(Dictionary<int, int> groupOrders) => 
        await _aditionalGroupRepository.UpdateDisplayOrdersAsync(groupOrders);

    public async Task<ProductAditionalGroupEntity> AssignToProductAsync(Guid productId, Guid aditionalGroupId, AssignAditionalGroupToProductRequestDto request)
    {
        // Verificar se produto existe
        var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value) ?? throw new ArgumentException("Produto não encontrado");

        // Verificar se grupo existe
        var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(aditionalGroupId, _currentUserService.GetTenantGuid()!.Value) ?? 
            throw new ArgumentException("Grupo de adicionais não encontrado");

        var productAditionalGroup = new ProductAditionalGroupEntity
        {
            ProductId = productId,
            AditionalGroupId = aditionalGroupId,
            DisplayOrder = request.DisplayOrder,
            IsRequired = request.IsRequired,
            MinSelectionsOverride = request.MinSelectionsOverride,
            MaxSelectionsOverride = request.MaxSelectionsOverride
        };
        return productAditionalGroup;
    }

    public async Task RemoveFromProductAsync(Guid productId, Guid aditionalGroupId)
    {
        //TODO:
        // Implementar remoção da associação produto-grupo
        // Esta implementação precisaria de um repositório específico para ProductAditionalGroup
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null) 
    {
        return await _aditionalGroupRepository.IsNameUniqueAsync(name, excludeId, _currentUserService.GetTenantGuid()!.Value);
    }

    public async Task<bool> CanDeleteAditionalGroupAsync(Guid id) => 
        !await _aditionalGroupRepository.HasAditionalsAsync(id);
}
