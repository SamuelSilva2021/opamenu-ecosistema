using System;
using System.ComponentModel.DataAnnotations;

namespace OpaMenu.Domain.DTOs.TenantCustomer
{
    public class UpdateTenantCustomerRequestDto
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }
        
        public string? Notes { get; set; }
    }
}
