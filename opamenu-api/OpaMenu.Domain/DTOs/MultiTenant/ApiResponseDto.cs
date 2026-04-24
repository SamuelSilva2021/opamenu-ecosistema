namespace OpaMenu.Domain.DTOs.MultiTenant;

public sealed class ApiResponseDto<T>
{
    public bool Succeeded { get; set; }
    public T Data { get; set; } = default!;
    public List<ErrorDto> Errors { get; set; } = [];
}
