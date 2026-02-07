using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.AditionalGroup;
using OpaMenu.Domain.DTOs.Aditionals;
using OpaMenu.Domain.DTOs.Product;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class ProductAditionalGroupService(
    IProductAditionalGroupRepository productAditionalGroupRepository, 
    IProductRepository productRepository, 
    IAditionalGroupRepository aditionalGroupRepository, 
    ILogger<ProductAditionalGroupService> logger,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IProductAditionalGroupService
{
    private readonly IProductAditionalGroupRepository _productAditionalGroupRepository = productAditionalGroupRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAditionalGroupRepository _aditionalGroupRepository = aditionalGroupRepository;
    private readonly ILogger<ProductAditionalGroupService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDTO<IEnumerable<ProductAditionalGroupResponseDto>>> GetProductAditionalGroupsAsync(Guid productId)
    {
        try
        {
            var productAditionalGroupsEntity = await _productAditionalGroupRepository.GetByProductIdAsync(productId);
            var aditionalGroups = _mapper.Map<IEnumerable<ProductAditionalGroupResponseDto>>(productAditionalGroupsEntity);
            return StaticResponseBuilder<IEnumerable<ProductAditionalGroupResponseDto>>.BuildOk(aditionalGroups);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductAditionalGroupResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductWithAditionalsResponseDto?>> GetProductWithAditionalsAsync(Guid productId)
    {
        try
        {            
            var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value);
            if (product == null)
                return StaticResponseBuilder<ProductWithAditionalsResponseDto?>.BuildOk(null);

            var aditionalGroups = await _productAditionalGroupRepository.GetByProductIdAsync(productId);
            _logger.LogInformation("Produto {ProductId} encontrado com {AditionalGroupsCount} grupos de adicionais", productId, aditionalGroups.Count());

           var productWithAditionalsDto = new ProductWithAditionalsResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                AditionalGroups = [.. aditionalGroups
                    .OrderBy(pag => pag.DisplayOrder)
                    .Select(pag => new AditionalGroupResponseDto
                    {
                        Id = pag.AditionalGroup.Id,
                        Name = pag.AditionalGroup.Name,
                        Description = pag.AditionalGroup.Description,
                        Type = pag.AditionalGroup.Type,
                        MinSelections = pag.MinSelectionsOverride ?? pag.AditionalGroup.MinSelections,
                        MaxSelections = pag.MaxSelectionsOverride ?? pag.AditionalGroup.MaxSelections,
                        IsRequired = pag.IsRequired,
                        DisplayOrder = pag.DisplayOrder,
                        Aditionals = [.. pag.AditionalGroup.Aditionals
                            .Where(a => a.IsActive)
                            .OrderBy(a => a.DisplayOrder)
                            .Select(a => new AditionalResponseDto
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Description = a.Description,
                                Price = a.Price,
                                ImageUrl = a.ImageUrl,
                                DisplayOrder = a.DisplayOrder,
                                AditionalGroupId = a.AditionalGroupId,
                                IsActive = a.IsActive
                            })]
                    })]
           };
            return StaticResponseBuilder<ProductWithAditionalsResponseDto?>.BuildOk(productWithAditionalsDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductWithAditionalsResponseDto?>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductAditionalGroupResponseDto>> AddAditionalGroupToProductAsync(Guid productId, AddProductAditionalGroupRequestDto request)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value) 
                ?? throw new ArgumentException($"Produto com ID {productId} não encontrado");

            var aditionalGroup = await _aditionalGroupRepository.GetByIdAsync(request.AditionalGroupId, _currentUserService.GetTenantGuid()!.Value) 
                ?? throw new ArgumentException($"Grupo de adicionais com ID {request.AditionalGroupId} não encontrado");

            var existingAssociation = await _productAditionalGroupRepository.ExistsAsync(productId, request.AditionalGroupId);
            if (existingAssociation)
                throw new InvalidOperationException("Este grupo de adicionais já está associado ao produto");

            var productAditionalGroup = new ProductAditionalGroupEntity
            {
                ProductId = productId,
                AditionalGroupId = request.AditionalGroupId,
                DisplayOrder = request.DisplayOrder,
                IsRequired = request.IsRequired,
                MinSelectionsOverride = request.MinSelectionsOverride,
                MaxSelectionsOverride = request.MaxSelectionsOverride
            };

            var productAditionalGroupEntity = await _productAditionalGroupRepository.AddAsync(productAditionalGroup);

            var productAditionalGroupDto = _mapper.Map<ProductAditionalGroupResponseDto>(productAditionalGroupEntity);

            return StaticResponseBuilder<ProductAditionalGroupResponseDto>.BuildOk(productAditionalGroupDto);

        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductAditionalGroupResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<ProductAditionalGroupResponseDto>> UpdateProductAditionalGroupAsync(Guid productId, Guid aditionalGroupId, UpdateProductAditionalGroupRequestDto request)
    {
        try
        {
            var productAditionalGroup = await _productAditionalGroupRepository.GetByProductAndAditionalGroupAsync(productId, aditionalGroupId);
            if (productAditionalGroup == null)
                throw new ArgumentException("Associação produto-grupo de adicionais não encontrada");

            productAditionalGroup.DisplayOrder = request.DisplayOrder;
            productAditionalGroup.IsRequired = request.IsRequired;
            productAditionalGroup.MinSelectionsOverride = request.MinSelectionsOverride;
            productAditionalGroup.MaxSelectionsOverride = request.MaxSelectionsOverride;

            await _productAditionalGroupRepository.UpdateAsync(productAditionalGroup);

            var productAditionalGroupDto = _mapper.Map<ProductAditionalGroupResponseDto>(productAditionalGroup);
            return StaticResponseBuilder<ProductAditionalGroupResponseDto>.BuildOk(productAditionalGroupDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<ProductAditionalGroupResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<object>> RemoveAditionalGroupFromProductAsync(Guid productId, Guid aditionalGroupId)
    {
        try
        {
            var productAditionalGroup = await _productAditionalGroupRepository.GetByProductAndAditionalGroupAsync(productId, aditionalGroupId)
                ?? throw new ArgumentException("Associação produto-grupo de adicionais não encontrada");

            await _productAditionalGroupRepository.DeleteAsync(productAditionalGroup);

            return StaticResponseBuilder<object>.BuildOk(new { });
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<object>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<IEnumerable<ProductAditionalGroupResponseDto>>> BulkAddAditionalGroupsToProductAsync(Guid productId, IEnumerable<AddProductAditionalGroupRequestDto> requests)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId, _currentUserService.GetTenantGuid()!.Value);
            if (product == null)
                throw new ArgumentException($"Produto com ID {productId} não encontrado");

            var aditionalGroupIds = requests.Select(r => r.AditionalGroupId).ToList();

            // Verificar se todos os grupos existem
            var existingGroups = new List<Guid>();
            foreach (var id in aditionalGroupIds)
            {
                var group = await _aditionalGroupRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
                if (group != null)
                    existingGroups.Add(id);
            }

            var missingGroups = aditionalGroupIds.Except(existingGroups).ToList();
            if (missingGroups.Any())
                throw new ArgumentException($"Grupos de adicionais não encontrados: {string.Join(", ", missingGroups)}");

            var existingAssociations = new List<Guid>();
            foreach (var id in aditionalGroupIds)
            {
                if (await _productAditionalGroupRepository.ExistsAsync(productId, id))
                    existingAssociations.Add(id);
            }

            var newAssociations = requests.Where(r => !existingAssociations.Contains(r.AditionalGroupId));

            var productAditionalGroups = newAssociations.Select(request => new ProductAditionalGroupEntity
            {
                ProductId = productId,
                AditionalGroupId = request.AditionalGroupId,
                DisplayOrder = request.DisplayOrder,
                IsRequired = request.IsRequired,
                MinSelectionsOverride = request.MinSelectionsOverride,
                MaxSelectionsOverride = request.MaxSelectionsOverride
            }).ToList();

            var result = await _productAditionalGroupRepository.AddRangeAsync(productAditionalGroups);
            var resultDto = _mapper.Map<IEnumerable<ProductAditionalGroupResponseDto>>(result);

            return StaticResponseBuilder<IEnumerable<ProductAditionalGroupResponseDto>>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductAditionalGroupResponseDto>>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> BulkRemoveAditionalGroupsFromProductAsync(Guid productId, IEnumerable<Guid> aditionalGroupIds)
    {
        try
        {
            var productAditionalGroups = new List<ProductAditionalGroupEntity>();
            foreach (var aditionalGroupId in aditionalGroupIds)
            {
                var pag = await _productAditionalGroupRepository.GetByProductAndAditionalGroupAsync(productId, aditionalGroupId);
                if (pag != null)
                    productAditionalGroups.Add(pag);
            }

            await _productAditionalGroupRepository.DeleteRangeAsync(productAditionalGroups);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<bool>> IsAditionalGroupAssignedToProductAsync(Guid productId, Guid aditionalGroupId)
    {
        try
        {
             await _productAditionalGroupRepository.ExistsAsync(productId, aditionalGroupId);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> CanRemoveAditionalGroupFromProductAsync(Guid productId, Guid aditionalGroupId)
    {
        // Verificar se há pedidos pendentes que usam este produto com adicionais
        // Por enquanto, retorna true - pode ser implementado futuramente
        return StaticResponseBuilder<bool>.BuildOk(true);
    }

    public async Task<ResponseDTO<IEnumerable<ProductDto>>> GetProductsWithAditionalGroupAsync(Guid aditionalGroupId)
    {
        try
        {
            var productsEntity = await _productAditionalGroupRepository.GetProductsByAditionalGroupAsync(aditionalGroupId);

            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(productsEntity);

            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildOk(productsDto);

        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<IEnumerable<ProductWithAditionalsResponseDto>>> GetAllProductsWithAditionalsAsync()
    {
        try
        {
            var products = await _productRepository.GetAllAsync(_currentUserService.GetTenantGuid()!.Value);
            var activeProducts = products.Where(p => p.IsActive);

            var result = new List<ProductWithAditionalsResponseDto>();

            foreach (var product in activeProducts)
            {
                var aditionalGroups = await _productAditionalGroupRepository.GetByProductIdAsync(product.Id);

                result.Add(new ProductWithAditionalsResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    AditionalGroups = aditionalGroups
                        .OrderBy(pag => pag.DisplayOrder)
                        .Select(pag => new AditionalGroupResponseDto
                        {
                            Id = pag.AditionalGroup.Id,
                            Name = pag.AditionalGroup.Name,
                            Description = pag.AditionalGroup.Description,
                            Type = pag.AditionalGroup.Type,
                            MinSelections = pag.MinSelectionsOverride ?? pag.AditionalGroup.MinSelections,
                            MaxSelections = pag.MaxSelectionsOverride ?? pag.AditionalGroup.MaxSelections,
                            IsRequired = pag.IsRequired,
                            DisplayOrder = pag.DisplayOrder,
                            Aditionals = pag.AditionalGroup.Aditionals
                                .Where(a => a.IsActive)
                                .OrderBy(a => a.DisplayOrder)
                                .Select(a => new AditionalResponseDto
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    Description = a.Description,
                                    Price = a.Price,
                                    ImageUrl = a.ImageUrl,
                                    DisplayOrder = a.DisplayOrder,
                                    AditionalGroupId = a.AditionalGroupId,
                                    IsActive = a.IsActive
                                }).ToList()
                        }).ToList()
                });
            }
            return StaticResponseBuilder<IEnumerable<ProductWithAditionalsResponseDto>>.BuildOk(result);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<IEnumerable<ProductWithAditionalsResponseDto>>.BuildErrorResponse(ex);
        }
        
    }

}
