using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpaMenu.Desktop.Models.DTOs.Api;

public class ResponseDto<T>
{
    [JsonPropertyName("succeeded")]
    public bool Succeeded { get; set; }

    [JsonPropertyName("errors")]
    public List<ApiErrorDto> Errors { get; set; } = new();

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
