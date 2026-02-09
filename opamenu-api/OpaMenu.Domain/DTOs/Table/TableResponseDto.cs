namespace OpaMenu.Domain.DTOs.Table;

public record TableResponseDto(
    Guid Id,
    string Name,
    int Capacity,
    bool IsActive,
    string? QrCodeUrl
);
