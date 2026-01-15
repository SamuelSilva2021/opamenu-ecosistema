using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AddonGroup;
using OpaMenu.Domain.DTOs.Addons;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;

namespace OpaMenu.Application.Services;

public class ProductAddonGroupService(
    IProductAddonGroupRepository productAddonGroupRepository, 
    IProductRepository productRepository, 
    IAddonGroupRepository addonGroupRepository, 
    ILogger<ProductAddonGroupService> logger,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IProductAddonGroupService
{
    private readonly IProductAddonGroupRepository _productAddonGroupRepository = productAddonGroupRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAddonGroupRepository _addonGroupRepository = addonGroupRepository;
    private readonly ILogger<ProductAddonGroupService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDTO<IEnumerable<ProductAddonGroupResponseDto>>> GetProductAddonGroupsAsync(int productId)
    {
        try
        {
            var productAddonGroupsEntity = await _productAddonGroupRepository.GetByProductIdAsync(productId);
            var addonGroups = _mapper.Map<IEnumerable<ProductAddonGroupResponseDto>>(productAddonGroupsEntity);
            return StaticResponseBuilder<IEnumerable<ProductAddonGroupResponseDto>>.BuildOk(addonGroups);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductAddonGroupResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductWithAddonsResponseDto?>> GetProductWithAddonsAsync(int productId)
    {
        try
        {            
            var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value);
            if (product == null)
                return StaticResponseBuilder<ProductWithAddonsResponseDto?>.BuildOk(null);

            var addonGroups = await _productAddonGroupRepository.GetByProductIdAsync(productId);
            _logger.LogInformation("Produto {ProductId} encontrado com {AddonGroupsCount} grupos de adicionais", productId, addonGroups.Count());

           var productWithAddonsDto = new ProductWithAddonsResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                AddonGroups = addonGroups
                    .OrderBy(pag => pag.DisplayOrder)
                    .Select(pag => new AddonGroupResponseDto
                    {
                        Id = pag.AddonGroup.Id,
                        Name = pag.AddonGroup.Name,
                        Description = pag.AddonGroup.Description,
                        Type = pag.AddonGroup.Type,
                        MinSelections = pag.MinSelectionsOverride ?? pag.AddonGroup.MinSelections,
                        MaxSelections = pag.MaxSelectionsOverride ?? pag.AddonGroup.MaxSelections,
                        IsRequired = pag.IsRequired,
                        DisplayOrder = pag.DisplayOrder,
                        Addons = pag.AddonGroup.Addons
                            .Where(a => a.IsActive)
                            .OrderBy(a => a.DisplayOrder)
                            .Select(a => new AddonResponseDto
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Description = a.Description,
                                Price = a.Price,
                                ImageUrl = a.ImageUrl,
                                DisplayOrder = a.DisplayOrder,
                                AddonGroupId = a.AddonGroupId,
                                IsActive = a.IsActive
                            }).ToList()
                    }).ToList()
            };
            return StaticResponseBuilder<ProductWithAddonsResponseDto?>.BuildOk(productWithAddonsDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductWithAddonsResponseDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductAddonGroupResponseDto>> AddAddonGroupToProductAsync(int productId, AddProductAddonGroupRequestDto request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value) 
                ?? throw new ArgumentException($"Produto com ID {productId} nÃ£o encontrado");

            var addonGroup = await _addonGroupRepository.GetByIdAsync(request.AddonGroupId, _currentUserService.GetTenantGuid()!.Value) 
                ?? throw new ArgumentException($"Grupo de adicionais com ID {request.AddonGroupId} nÃ£o encontrado");

            var existingAssociation = await _productAddonGroupRepository.ExistsAsync(productId, request.AddonGroupId);
            if (existingAssociation)
                throw new InvalidOperationException("Este grupo de adicionais jÃ¡ estÃ¡ associado ao produto");

            var productAddonGroup = new ProductAddonGroupEntity
            {
                ProductId = productId,
                AddonGroupId = request.AddonGroupId,
                DisplayOrder = request.DisplayOrder,
                IsRequired = request.IsRequired,
                MinSelectionsOverride = request.MinSelectionsOverride,
                MaxSelectionsOverride = request.MaxSelectionsOverride
            };

            var productAddonGroupEntity = await _productAddonGroupRepository.AddAsync(productAddonGroup);

            var productAddonGroupDto = _mapper.Map<ProductAddonGroupResponseDto>(productAddonGroupEntity);

            return StaticResponseBuilder<ProductAddonGroupResponseDto>.BuildOk(productAddonGroupDto);

        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductAddonGroupResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductAddonGroupResponseDto>> UpdateProductAddonGroupAsync(int productId, int addonGroupId, UpdateProductAddonGroupRequestDto request)
    {
        try
        {
            var productAddonGroup = await _productAddonGroupRepository.GetByProductAndAddonGroupAsync(productId, addonGroupId);
            if (productAddonGroup == null)
                throw new ArgumentException("AssociaÃ§Ã£o produto-grupo de adicionais nÃ£o encontrada");

            productAddonGroup.DisplayOrder = request.DisplayOrder;
            productAddonGroup.IsRequired = request.IsRequired;
            productAddonGroup.MinSelectionsOverride = request.MinSelectionsOverride;
            productAddonGroup.MaxSelectionsOverride = request.MaxSelectionsOverride;

            await _productAddonGroupRepository.UpdateAsync(productAddonGroup);

            var productAddonGroupDto = _mapper.Map<ProductAddonGroupResponseDto>(productAddonGroup);
            return StaticResponseBuilder<ProductAddonGroupResponseDto>.BuildOk(productAddonGroupDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductAddonGroupResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<object>> RemoveAddonGroupFromProductAsync(int productId, int addonGroupId)
    {
        try
        {
            var productAddonGroup = await _productAddonGroupRepository.GetByProductAndAddonGroupAsync(productId, addonGroupId)
                ?? throw new ArgumentException("AssociaÃ§Ã£o produto-grupo de adicionais nÃ£o encontrada");

            await _productAddonGroupRepository.DeleteAsync(productAddonGroup);

            return StaticResponseBuilder<object>.BuildOk(new { });
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<object>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<object>> ReorderProductAddonGroupsAsync(int productId, Dictionary<int, int> groupOrders)
    {
        try
        {
            var productAddonGroups = await _productAddonGroupRepository.GetByProductIdAsync(productId);

            foreach (var pag in productAddonGroups)
            {
                if (groupOrders.TryGetValue(pag.AddonGroupId, out int newOrder))
                {
                    pag.DisplayOrder = newOrder;
                }
            }

            await _productAddonGroupRepository.SaveChangesAsync();
            return StaticResponseBuilder<object>.BuildOk(new { });
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<object>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<IEnumerable<ProductAddonGroupResponseDto>>> BulkAddAddonGroupsToProductAsync(int productId, IEnumerable<AddProductAddonGroupRequestDto> requests)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value);
            if (product == null)
                throw new ArgumentException($"Produto com ID {productId} nÃ£o encontrado");

            var addonGroupIds = requests.Select(r => r.AddonGroupId).ToList();

            // Verificar se todos os grupos existem
            var existingGroups = new List<int>();
            foreach (var id in addonGroupIds)
            {
                var group = await _addonGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
                if (group != null)
                    existingGroups.Add(id);
            }

            var missingGroups = addonGroupIds.Except(existingGroups).ToList();
            if (missingGroups.Any())
                throw new ArgumentException($"Grupos de adicionais nÃ£o encontrados: {string.Join(", ", missingGroups)}");

            // Verificar se jÃ¡ nÃ£o estÃ£o associados
            var existingAssociations = new List<int>();
            foreach (var id in addonGroupIds)
            {
                if (await _productAddonGroupRepository.ExistsAsync(productId, id))
                    existingAssociations.Add(id);
            }

            var newAssociations = requests.Where(r => !existingAssociations.Contains(r.AddonGroupId));

            var productAddonGroups = newAssociations.Select(request => new ProductAddonGroupEntity
            {
                ProductId = productId,
                AddonGroupId = request.AddonGroupId,
                DisplayOrder = request.DisplayOrder,
                IsRequired = request.IsRequired,
                MinSelectionsOverride = request.MinSelectionsOverride,
                MaxSelectionsOverride = request.MaxSelectionsOverride
            }).ToList();

            var result = await _productAddonGroupRepository.AddRangeAsync(productAddonGroups);
            var resultDto = _mapper.Map<IEnumerable<ProductAddonGroupResponseDto>>(result);

            return StaticResponseBuilder<IEnumerable<ProductAddonGroupResponseDto>>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductAddonGroupResponseDto>>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> BulkRemoveAddonGroupsFromProductAsync(int productId, IEnumerable<int> addonGroupIds)
    {
        try
        {
            var productAddonGroups = new List<ProductAddonGroupEntity>();
            foreach (var addonGroupId in addonGroupIds)
            {
                var pag = await _productAddonGroupRepository.GetByProductAndAddonGroupAsync(productId, addonGroupId);
                if (pag != null)
                    productAddonGroups.Add(pag);
            }

            await _productAddonGroupRepository.DeleteRangeAsync(productAddonGroups);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<bool>> IsAddonGroupAssignedToProductAsync(int productId, int addonGroupId)
    {
        try
        {
             await _productAddonGroupRepository.ExistsAsync(productId, addonGroupId);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> CanRemoveAddonGroupFromProductAsync(int productId, int addonGroupId)
    {
        // Verificar se hÃ¡ pedidos pendentes que usam este produto com adicionais
        var ordersUsingProductWithAddons = false; // SimulaÃ§Ã£o de verificaÃ§Ã£o
        // Por enquanto, retorna true - pode ser implementado futuramente
        return StaticResponseBuilder<bool>.BuildOk(true);
    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsWithAddonGroupAsync(int addonGroupId)
    {
        try
        {
            var productsEntity = await _productAddonGroupRepository.GetProductsByAddonGroupAsync(addonGroupId);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);

            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(productsDto);

        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<ProductWithAddonsResponseDto>>> GetAllProductsWithAddonsAsync()
    {
        try
        {
            var products = await _productRepository.GetAllAsync(_currentUserService.GetTenantGuid()!.Value);
            var activeProducts = products.Where(p => p.IsActive);

            var result = new List<ProductWithAddonsResponseDto>();

            foreach (var product in activeProducts)
            {
                var addonGroups = await _productAddonGroupRepository.GetByProductIdAsync(product.Id);

                result.Add(new ProductWithAddonsResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    AddonGroups = addonGroups
                        .OrderBy(pag => pag.DisplayOrder)
                        .Select(pag => new AddonGroupResponseDto
                        {
                            Id = pag.AddonGroup.Id,
                            Name = pag.AddonGroup.Name,
                            Description = pag.AddonGroup.Description,
                            Type = pag.AddonGroup.Type,
                            MinSelections = pag.MinSelectionsOverride ?? pag.AddonGroup.MinSelections,
                            MaxSelections = pag.MaxSelectionsOverride ?? pag.AddonGroup.MaxSelections,
                            IsRequired = pag.IsRequired,
                            DisplayOrder = pag.DisplayOrder,
                            Addons = pag.AddonGroup.Addons
                                .Where(a => a.IsActive)
                                .OrderBy(a => a.DisplayOrder)
                                .Select(a => new AddonResponseDto
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    Description = a.Description,
                                    Price = a.Price,
                                    ImageUrl = a.ImageUrl,
                                    DisplayOrder = a.DisplayOrder,
                                    AddonGroupId = a.AddonGroupId,
                                    IsActive = a.IsActive
                                }).ToList()
                        }).ToList()
                });
            }
            return StaticResponseBuilder<IEnumerable<ProductWithAddonsResponseDto>>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductWithAddonsResponseDto>>.BuildErrorResponse(ex);
        }
        
    }

}


