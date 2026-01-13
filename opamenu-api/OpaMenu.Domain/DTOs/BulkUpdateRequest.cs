using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs
{
    public class BulkUpdateRequest
    {
        [Required]
        public List<int> ProductIds { get; set; } = new();
        
        [Required]
        public bool SetActive { get; set; } // true to make active, false to make inactive
    }
}