
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Application.Extensions;

/// <summary>
/// Extension methods para operações elegantes com produtos
/// Utiliza recursos modernos do C# 13 e .NET 9
/// </summary>
public static class ProductExtensions
{
    /// <summary>
    /// Atualiza as propriedades bÃ¡sicas do produto de forma fluente
    /// </summary>
    /// <param name="product">Produto a ser atualizado</param>
    /// <param name="name">Nome do produto</param>
    /// <param name="description">DescriÃ§Ã£o do produto</param>
    /// <param name="price">PreÃ§o do produto</param>
    /// <returns>O prÃ³prio produto para encadeamento fluente</returns>
    public static ProductEntity UpdateBasicInfo(this ProductEntity product, string name, string? description, decimal price)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.UpdatedAt = DateTime.UtcNow;
        
        return product;
    }

    /// <summary>
    /// Atualiza a categoria do produto
    /// </summary>
    /// <param name="product">Produto a ser atualizado</param>
    /// <param name="categoryId">ID da nova categoria</param>
    /// <returns>O prÃ³prio produto para encadeamento fluente</returns>
    public static ProductEntity UpdateCategory(this ProductEntity product, Guid categoryId)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.CategoryId = categoryId;
        product.UpdatedAt = DateTime.UtcNow;
        
        return product;
    }

    /// <summary>
    /// Atualiza a URL da imagem do produto
    /// </summary>
    /// <param name="product">Produto a ser atualizado</param>
    /// <param name="imageUrl">Nova URL da imagem</param>
    /// <returns>O prÃ³prio produto para encadeamento fluente</returns>
    public static ProductEntity UpdateImage(this ProductEntity product, string? imageUrl)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.ImageUrl = imageUrl;
        product.UpdatedAt = DateTime.UtcNow;
        
        return product;
    }

    /// <summary>
    /// Atualiza o status de ativaÃ§Ã£o do produto
    /// </summary>
    /// <param name="product">Produto a ser atualizado</param>
    /// <param name="isActive">Status de ativaÃ§Ã£o</param>
    /// <returns>O prÃ³prio produto para encadeamento fluente</returns>
    public static ProductEntity UpdateStatus(this ProductEntity product, bool isActive)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;
        
        return product;
    }

    /// <summary>
    /// Atualiza a ordem de exibiÃ§Ã£o do produto
    /// </summary>
    /// <param name="product">Produto a ser atualizado</param>
    /// <param name="displayOrder">Nova ordem de exibiÃ§Ã£o</param>
    /// <returns>O prÃ³prio produto para encadeamento fluente</returns>
    public static ProductEntity UpdateDisplayOrder(this ProductEntity product, int displayOrder)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        if (displayOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(displayOrder));

        product.DisplayOrder = displayOrder;
        product.UpdatedAt = DateTime.UtcNow;
        
        return product;
    }

    /// <summary>
    /// Atualiza apenas o preÃ§o do produto (operaÃ§Ã£o rÃ¡pida)
    /// </summary>
    /// <param name="product">Produto a ser atualizado</param>
    /// <param name="newPrice">Novo preÃ§o</param>
    /// <returns>O prÃ³prio produto para encadeamento fluente</returns>
    public static ProductEntity UpdatePrice(this ProductEntity product, decimal newPrice)
    {
        ArgumentNullException.ThrowIfNull(product);
        
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(newPrice));

        product.Price = newPrice;
        product.UpdatedAt = DateTime.UtcNow;
        
        return product;
    }

    /// <summary>
    /// Verifica se o produto é válido de acordo com as regras de negócio
    /// </summary>
    /// <param name="product">Produto a ser validado</param>
    /// <returns>True se vÃ¡lido, caso contrÃ¡rio lanÃ§a exceÃ§Ã£o</returns>
    /// <exception cref="ArgumentException">Produto invÃ¡lido</exception>
    public static bool IsValid(this ProductEntity product)
    {
        ArgumentNullException.ThrowIfNull(product);

        var guidDefault = new Guid();

        return product switch
        {
            { Name: null or "" } => throw new ArgumentException("Product name is required"),
            { Name.Length: > 100 } => throw new ArgumentException("Product name cannot exceed 100 characters"),
            { Description.Length: > 1000 } => throw new ArgumentException("Product description cannot exceed 1000 characters"),
            { Price: <= 0 } => throw new ArgumentException("Product price must be greater than zero"),
            { DisplayOrder: < 0 } => throw new ArgumentException("Display order cannot be negative"),
            _ => true
        };
    }

    /// <summary>
    /// Cria uma cÃ³pia do produto com novos valores (imutabilidade)
    /// </summary>
    /// <param name="product">Produto original</param>
    /// <param name="updates">AÃ§Ã£o para aplicar atualizaÃ§Ãµes</param>
    /// <returns>Nova instÃ¢ncia do produto com as atualizaÃ§Ãµes</returns>
    public static ProductEntity WithUpdates(this ProductEntity product, Action<ProductEntity> updates)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(updates);

        var updatedProduct = new ProductEntity
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            DisplayOrder = product.DisplayOrder,
            CreatedAt = product.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            Category = product.Category
        };

        updates(updatedProduct);
        updatedProduct.IsValid(); // Validar apÃ³s as atualizaÃ§Ãµes
        
        return updatedProduct;
    }
}
