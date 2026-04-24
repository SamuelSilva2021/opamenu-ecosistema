using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Application.Models
{
    // Story 2.3: Quick Operations DTOs
    public class BulkUpdateRequest
    {
        [Required]
        public List<int> ProductIds { get; set; } = new();
        
        [Required]
        public string Operation { get; set; } = string.Empty; // "toggle-availability", "update-price", "change-category"
        
        public object? Value { get; set; } // Price for price updates, CategoryId for category changes
    }
}
