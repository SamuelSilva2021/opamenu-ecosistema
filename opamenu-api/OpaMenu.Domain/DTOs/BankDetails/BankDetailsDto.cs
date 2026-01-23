using System;

namespace OpaMenu.Domain.DTOs.BankDetails
{
    public class BankDetailsDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string? BankName { get; set; }
        public string? Agency { get; set; }
        public string? AccountNumber { get; set; }
        public int? AccountType { get; set; }
        public int? BankId { get; set; }
        public string? PixKey { get; set; }
        public bool IsPixKeySelected { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
