using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using Opamenu.Domain.DTOs;

namespace Opamenu.Domain.DTOs.Tab;

public record TabResponseDto(
    Guid Id,
    Guid TableId,
    string? Name,
    ETabStatus Status,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    IEnumerable<OrderResponseDto>? Orders = null
);

