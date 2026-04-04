using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpaMenu.Desktop.Models.Enums;

namespace OpaMenu.Desktop.Models.DTOs;

public class CashShiftDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("openedAt")]
    public DateTime OpenedAt { get; set; }

    [JsonPropertyName("closedAt")]
    public DateTime? ClosedAt { get; set; }

    [JsonPropertyName("openingBalance")]
    public decimal OpeningBalance { get; set; }

    [JsonPropertyName("closingBalance")]
    public decimal? ClosingBalance { get; set; }

    [JsonPropertyName("expectedBalance")]
    public decimal ExpectedBalance { get; set; }

    [JsonPropertyName("status")]
    public ECashShiftStatus Status { get; set; }

    [JsonPropertyName("movements")]
    public List<CashMovementDto> Movements { get; set; } = new();
}
