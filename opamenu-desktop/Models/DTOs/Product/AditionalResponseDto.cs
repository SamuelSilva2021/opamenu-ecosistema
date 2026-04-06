using System;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Aditional;

public class AditionalResponseDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }

    [JsonPropertyName("aditionalGroupId")]
    public Guid AditionalGroupId { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}