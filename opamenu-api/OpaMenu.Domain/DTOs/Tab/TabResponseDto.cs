using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Domain.DTOs;

namespace OpaMenu.Domain.DTOs.Tab;

public record TabResponseDto(
    Guid Id,
    Guid TableId,
    string? Name,
    ETabStatus Status,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    IEnumerable<OrderResponseDto>? Orders = null
);

