using System.ComponentModel.DataAnnotations;
using Opamenu.Infrastructure.Shared.Enums.Opamenu;

namespace Opamenu.Domain.DTOs
{
    public class CancelOrderRequestDto
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}


