using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Table;

public record CreateTableRequestDto(
    [Required(ErrorMessage = "O nome da mesa é obrigatório")]
    [MaxLength(50, ErrorMessage = "O nome da mesa deve ter no máximo 50 caracteres")]
    string Name,

    [Range(1, 100, ErrorMessage = "A capacidade deve ser entre 1 e 100")]
    int Capacity
);
