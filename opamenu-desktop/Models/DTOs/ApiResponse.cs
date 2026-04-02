using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs;

/// <summary>
/// Envelope de resposta padrão do ecossistema OpaMenu
/// Baseado na classe ResponseDTO do Authenticator.API
/// </summary>
public class ApiResponse<T>
{
    [JsonPropertyName("succeeded")]
    public bool Succeeded { get; set; }

    [JsonPropertyName("successResult")]
    public string? SuccessResult { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}