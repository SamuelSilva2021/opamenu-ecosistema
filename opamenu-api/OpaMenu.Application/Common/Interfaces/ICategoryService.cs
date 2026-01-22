
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Category;

namespace OpaMenu.Application.Common.Interfaces;

public interface ICategoryService
{
    Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync();
    Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesAsync();
    Task<ResponseDTO<CategoryResponseDto?>> GetCategoryByIdAsync(Guid id);
    Task<ResponseDTO<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createDto);
    Task<ResponseDTO<CategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto updateDto);
    Task<ResponseDTO<bool>> DeleteCategoryAsync(Guid id);
    Task<bool> CanDeleteCategoryAsync(Guid id);
    Task<ResponseDTO<CategoryResponseDto>> ToggleCategoryActiveAsync(Guid id);
    Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesBySlugAsync(string slug);
}