using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs;

public class UpdateAditionalRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero")]
    public decimal Price { get; set; }
    
    [Required]
    public Guid AditionalGroupId { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool IsActive { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
}
