using OpaMenu.Infrastructure.Shared.Entities;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Web.Models.DTOs
{
    public class UpdateOrderStatusRequestDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}

