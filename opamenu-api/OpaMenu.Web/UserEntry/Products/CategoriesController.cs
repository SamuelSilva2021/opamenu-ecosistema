using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Common.Builders;
using OpaMenu.Application.Common.Interfaces;
using OpaMenu.Application.Features.Categories;
using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs.Category;
using OpaMenu.Infrastructure.Anotations;

namespace OpaMenu.Web.UserEntry.Products;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : FoodBaseController
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoryService categoryService,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Obter todas as categorias ordenadas por DisplayOrder
    /// </summary>
    [HttpGet]
    [MapPermission(MODULE_CATEGORY, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<IEnumerable<CategoryResponseDto>>>> GetCategories()
    {
        var response = await _categoryService.GetAllCategoriesAsync();
        return BuildResponse(response);
    }

    /// <summary>
    /// Obter apenas categorias ativas
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<CategoryResponseDto>>>> GetActiveCategories()
    {
        var response = await _categoryService.GetActiveCategoriesAsync();
        return BuildResponse(response);
    }

    /// <summary>
    /// Obter categoria por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDTO<CategoryResponseDto>>> GetCategory(int id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);
        return BuildResponse(response);
    }

    /// <summary>
    /// Criar nova categoria
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ResponseDTO<CategoryResponseDto>>> CreateCategory([FromBody] CreateCategoryRequestDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(StaticResponseBuilder<CategoryResponseDto>.BuildError(errors));
        }

        var response = await _categoryService.CreateCategoryAsync(createDto);
        return BuildResponse(response);
    }

    /// <summary>
    /// Atualizar categoria existente
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<ActionResult<ResponseDTO<CategoryResponseDto>>> UpdateCategory(int id, [FromBody] UpdateCategoryRequestDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(StaticResponseBuilder<CategoryResponseDto>.BuildError(errors));
        }

        var response = await _categoryService.UpdateCategoryAsync(id, updateDto);
        return BuildResponse(response);
    }

    /// <summary>
    /// Excluir categoria por ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDTO<bool>>> DeleteCategory(int id)
    {
        var response = await _categoryService.DeleteCategoryAsync(id);
        return BuildResponse(response);
    }

    /// <summary>
    /// Alternar status ativo/inativo de uma categoria
    /// </summary>
    [HttpPut("{id}/toggle-active")]
    public async Task<ActionResult<ResponseDTO<CategoryResponseDto>>> ToggleCategoryActive(int id)
    {
        var response = await _categoryService.ToggleCategoryActiveAsync(id);
        return BuildResponse(response);
    }

    /// <summary>
    /// Verificar se categoria pode ser excluída
    /// </summary>
    [HttpGet("{id}/can-delete")]
    public async Task<ActionResult<ResponseDTO<object>>> CanDeleteCategory(int id)
    {
        var canDelete = await _categoryService.CanDeleteCategoryAsync(id);
        return Ok(StaticResponseBuilder<object>.BuildOk(new { canDelete }));
    }
}
