using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Application.DTOs.Table;

public record TableRequestDto(
    [Required] [MaxLength(50)] string Name,
    [Required] int Capacity,
    bool IsActive = true
);
