using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;

namespace Opamenu.Web.Models.DTOs
{
    public class UpdatEOrderStatusRequestDto
    {
        [Required]
        public EOrderStatus Status { get; set; }
    }
}

