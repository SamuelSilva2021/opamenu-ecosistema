using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace Opamenu.Domain.DTOs
{
    /// <summary>
    /// DTO para atualizar o status de um pedido
    /// </summary>
    public class UpdatEOrderStatusRequestDto
    {
        [Required]
        public EOrderStatus Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public Guid? DriverId { get; set; }

        [StringLength(50)]
        public Guid? UserId { get; set; }
    }
}
