
using OpaMenu.Application.DTOs;
using OpaMenu.Application.Features.Categories;
using OpaMenu.Domain.DTOs.Category;

namespace OpaMenu.Application.Common.Interfaces;

public interface ICategoryService
{
    Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync();
    Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesAsync();
    Task<ResponseDTO<CategoryResponseDto?>> GetCategoryByIdAsync(int id);
    Task<ResponseDTO<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryRequestDto createDto);
    Task<ResponseDTO<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryRequestDto updateDto);
    Task<ResponseDTO<bool>> DeleteCategoryAsync(int id);
    Task<bool> CanDeleteCategoryAsync(int id);
    Task<ResponseDTO<CategoryResponseDto>> ToggleCategoryActiveAsync(int id);
    Task<ResponseDTO<IEnumerable<CategoryResponseDto>>> GetActiveCategoriesBySlugAsync(string slug);
}