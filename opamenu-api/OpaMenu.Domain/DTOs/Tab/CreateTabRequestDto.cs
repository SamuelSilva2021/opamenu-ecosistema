using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Tab;

public record CreateTabRequestDto(
    [MaxLength(50, ErrorMessage = "O nome da comanda deve ter no máximo 50 caracteres")]
    string? Name = null
);

