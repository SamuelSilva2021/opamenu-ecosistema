using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs;

public class ProductDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("aditionalGroups")]
    public List<AditionalGroupResponseDto>? AditionalGroups { get; set; }
}