using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Common.Builders;
using OpaMenu.Application.Common.Interfaces;
using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;

namespace OpaMenu.Application.Features.Categories;

public class CategoryService(
    ICategoryRepository categoryRepository,
    ILogger<CategoryService> logger,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ITenantRepository tenantRepository
    ) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly ILogger<CategoryService> _logger = logger;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IMapper _mapper = mapper;
    private readonly ITenantRepository _tenantRepository = tenantRepository;

    public async Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync()
    {
        try
        {
            var categoriesEntity = await _categoryRepository.GetCategoriesOrderedAsync(_currentUserService.GetTenantGuid()!.Value) ??
                throw new ArgumentException("Erro ao obter categorias");

            var categories = _mapper.Map<IEnumerable<CategoryResponseDto>>(categoriesEntity);

            return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildOk(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as categorias");
            return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetActiveCategoriesAsync(_currentUserService.GetTenantGuid()!.Value) ??
                throw new ArgumentException("Erro ao obter categorias ativas");

            var dtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categorias ativas");
            return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildErrorResponse(ex);
        }

    }

    public async Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesBySlugAsync(string slug)
    {
        try
        {
            var tenant = await _tenantRepository.GetBySlugAsync(slug);
            if (tenant == null)
                return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildError("Loja nÃ£o encontrada");

            var categories = await _categoryRepository.GetActiveCategoriesAsync(tenant.Id);

            var dtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categorias ativas por slug: {Slug}", slug);
            return StaticResponseBuilder<IEnumerable<CategoryResponseDto>>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<CategoryResponseDto?>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);

            if (category == null)
                return StaticResponseBuilder<CategoryResponseDto?>.BuildError("Categoria nÃ£o encontrada");

            var dto = _mapper.Map<CategoryResponseDto>(category);
            return StaticResponseBuilder<CategoryResponseDto?>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categoria por ID: {CategoryId}", id);
            return StaticResponseBuilder<CategoryResponseDto?>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createDto)
    {
        try
        {
            if (await _categoryRepository.IsNameUniqueAsync(_currentUserService.GetTenantGuid()!.Value, createDto.Name))
                return StaticResponseBuilder<CategoryResponseDto>.BuildError($"JÃ¡ existe uma categoria com o nome '{createDto.Name}'");

            var category = _mapper.Map<CategoryEntity>(createDto);

            if (category.DisplayOrder == 0 && createDto.DisplayOrder == 0)
                category.DisplayOrder = await _categoryRepository.GetNextDisplayOrderAsync();

            category.TenantId = _currentUserService.GetTenantGuid()!.Value;

            var createdCategory = await _categoryRepository.AddAsync(category);

            _logger.LogInformation("Categoria criada: {CategoryName} (ID: {CategoryId})",
                createdCategory.Name, createdCategory.Id);

            var dto = _mapper.Map<CategoryResponseDto>(createdCategory);
            return StaticResponseBuilder<CategoryResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Erro ao criar categoria: {@CreateDto}", createDto);
            return StaticResponseBuilder<CategoryResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryRequestDto updateDto)
    {
        try
        {
            _logger.LogInformation("UpdateCategoryAsync called - ID: {Id}, UpdateDto: {@UpdateDto}", id, updateDto);

            var category = await _categoryRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (category == null)
                return StaticResponseBuilder<CategoryResponseDto>.BuildError($"Categoria com ID {id} nÃ£o encontrada");

            if (await _categoryRepository.IsNameUniqueAsync(_currentUserService.GetTenantGuid()!.Value, updateDto.Name, id))
                return StaticResponseBuilder<CategoryResponseDto>.BuildError($"JÃ¡ existe uma categoria com o nome '{updateDto.Name}'");

            _mapper.Map(updateDto, category);

            await _categoryRepository.UpdateAsync(category);

            _logger.LogInformation("Categoria atualizada: {CategoryName} (ID: {CategoryId})",
                category.Name, category.Id);

            var dto = _mapper.Map<CategoryResponseDto>(category);
            return StaticResponseBuilder<CategoryResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar categoria com ID {CategoryId}: {@UpdateDto}", id, updateDto);
            return StaticResponseBuilder<CategoryResponseDto>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<ResponseDTO<bool>> DeleteCategoryAsync(int id)
    {
        try
        {
            if (!await CanDeleteCategoryAsync(id))
            {
                return StaticResponseBuilder<bool>.BuildError("NÃ£o Ã© possÃ­vel excluir categoria que possui produtos associados");
            }

            var categoryEntity = await _categoryRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (categoryEntity == null)
                return StaticResponseBuilder<bool>.BuildError("Categoria nÃ£o encontrada");

            await _categoryRepository.DeleteAsync(categoryEntity);

            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir categoria com ID {CategoryId}", id);
            return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
        
    }

    public async Task<bool> CanDeleteCategoryAsync(int id) => !await _categoryRepository.HasProductsAsync(id);

    public async Task<ResponseDTO<CategoryResponseDto>> ToggleCategoryActiveAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id, _currentUserService.GetTenantGuid()!.Value);
            if (category == null)
            {
                return StaticResponseBuilder<CategoryResponseDto>.BuildError($"Categoria com ID {id} nÃ£o encontrada");
            }

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            _logger.LogInformation("Status da categoria alterado: {CategoryName} (ID: {CategoryId}) - Ativa: {IsActive}",
                category.Name, category.Id, category.IsActive);

            var dto = _mapper.Map<CategoryResponseDto>(category);
            return StaticResponseBuilder<CategoryResponseDto>.BuildOk(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status da categoria com ID {CategoryId}", id);
            return StaticResponseBuilder<CategoryResponseDto>.BuildErrorResponse(ex);
        }
        
    }
}

