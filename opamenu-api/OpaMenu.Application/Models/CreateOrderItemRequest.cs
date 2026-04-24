using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Application.Models;

public class CreateOrderItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
