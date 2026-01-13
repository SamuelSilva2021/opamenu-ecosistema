using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Category;

public class CreateCategoryRequestDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
    
    public bool IsActive { get; set; } = true;
}
