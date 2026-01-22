using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Web.Models.DTOs
{
    public class UpdatEOrderStatusRequestDto
    {
        [Required]
        public EOrderStatus Status { get; set; }
    }
}

