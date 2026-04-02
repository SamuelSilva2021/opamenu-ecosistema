using System.Collections.Generic;
using System.Threading.Tasks;
using OpaMenu.Desktop.Models.DTOs;

namespace OpaMenu.Desktop.Services;

public interface ICatalogService
{
    /// <summary>
    /// Busca as categorias da loja na opamenu-api através do endpoint protegido /api/Categories
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();

    /// <summary>
    /// Busca os produtos da loja na opamenu-api através do endpoint protegido /api/products
    /// </summary>
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}