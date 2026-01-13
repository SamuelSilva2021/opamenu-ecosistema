using OpaMenu.Infrastructure.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Application.Models;

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

