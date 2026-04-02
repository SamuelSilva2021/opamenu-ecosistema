using System;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs;

public class CategoryDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}