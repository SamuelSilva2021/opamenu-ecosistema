namespace OpaMenu.Application.DTOs.Table;

public record TableResponseDto(
    Guid Id,
    string Name,
    int Capacity,
    bool IsActive,
    string? QrCodeUrl,
    Guid TenantId
);
