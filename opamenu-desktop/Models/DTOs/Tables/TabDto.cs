using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Tables;

public class TabDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("tableId")]
    public Guid TableId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("openedAt")]
    public DateTime OpenedAt { get; set; }

    [JsonPropertyName("closedAt")]
    public DateTime? ClosedAt { get; set; }

    [JsonPropertyName("orders")]
    public List<OrderDto>? Orders { get; set; }
}
