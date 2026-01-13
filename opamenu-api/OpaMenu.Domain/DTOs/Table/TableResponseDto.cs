namespace OpaMenu.Domain.DTOs.Table;

public record TableResponseDto(
    int Id,
    string Name,
    int Capacity,
    bool IsActive,
    string? QrCodeUrl
);
