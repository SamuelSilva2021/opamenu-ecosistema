using System.ComponentModel.DataAnnotations;

namespace Opamenu.Application.DTOs.Table;

public record TableRequestDto(
    [Required] [MaxLength(50)] string Name,
    [Required] int Capacity,
    bool IsActive = true
);
