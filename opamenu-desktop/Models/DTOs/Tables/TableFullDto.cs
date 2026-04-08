using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Tables;

public class TableFullDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("qrCodeUrl")]
    public string? QrCodeUrl { get; set; }

    [JsonPropertyName("layoutX")]
    public double LayoutX { get; set; }

    [JsonPropertyName("layoutY")]
    public double LayoutY { get; set; }

    [JsonPropertyName("layoutWidth")]
    public double LayoutWidth { get; set; }

    [JsonPropertyName("layoutHeight")]
    public double LayoutHeight { get; set; }

    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    [JsonPropertyName("tabs")]
    public List<TabDto> Tabs { get; set; } = new();
}
