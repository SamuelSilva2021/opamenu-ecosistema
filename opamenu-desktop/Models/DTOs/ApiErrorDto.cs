using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs;

public class ApiErrorDto
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("property")]
    public string? Property { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public List<string>? Details { get; set; }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Property))
            return $"{Property} - {Message}";

        return Message;
    }
}
