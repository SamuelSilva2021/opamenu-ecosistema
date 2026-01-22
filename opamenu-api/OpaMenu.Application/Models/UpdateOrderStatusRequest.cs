using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Application.Models;

public class UpdatEOrderStatusRequest
{
    [Required]
    public EOrderStatus Status { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

