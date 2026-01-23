using System;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.BankDetails
{
    public class CreateBankDetailsRequestDto
    {
        [MaxLength(100)]
        public string? BankName { get; set; }
        
        [MaxLength(20)]
        public string? Agency { get; set; }
        
        [MaxLength(30)]
        public string? AccountNumber { get; set; }
        
        public int? AccountType { get; set; }
        
        public int? BankId { get; set; }
        
        [MaxLength(100)]
        public string? PixKey { get; set; }
        
        public bool IsPixKeySelected { get; set; } = false;
    }
}
