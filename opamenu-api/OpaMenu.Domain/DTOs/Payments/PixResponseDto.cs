using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Payments
{
    public class PixResponseDto
    {
        public string PixId { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
