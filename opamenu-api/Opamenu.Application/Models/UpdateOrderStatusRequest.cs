using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace Opamenu.Application.Models;

public class UpdatEOrderStatusRequest
{
    [Required]
    public EOrderStatus Status { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

