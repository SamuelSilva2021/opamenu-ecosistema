using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.DTOs.Aditional;

public class AditionalGroupResponseDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("type")]
    public EAditionalGroupType Type { get; set; }

    [JsonPropertyName("minSelections")]
    public int? MinSelections { get; set; }

    [JsonPropertyName("maxSelections")]
    public int? MaxSelections { get; set; }

    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("aditionals")]
    public List<AditionalResponseDto> Aditionals { get; set; } = new();
}