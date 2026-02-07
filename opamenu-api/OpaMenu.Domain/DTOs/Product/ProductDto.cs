using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }
        
        [Required]
        public Guid CategoryId { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int DisplayOrder { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        // Category information
        public string CategoryName { get; set; } = string.Empty;
        
        // Aditional Groups information
        public List<ProductAditionalGroupResponseDto> AditionalGroups { get; set; } = new();
    }
}