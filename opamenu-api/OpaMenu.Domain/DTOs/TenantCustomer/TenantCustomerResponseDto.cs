using System;
using OpaMenu.Domain.DTOs.Customer;

namespace OpaMenu.Domain.DTOs.TenantCustomer
{
    public class TenantCustomerResponseDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid CustomerId { get; set; }
        public string? DisplayName { get; set; }
        public string? Notes { get; set; }
        public DateTime? FirstPurchaseAt { get; set; }
        public DateTime? LastPurchaseAt { get; set; }
        public decimal TotalOrders { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CustomerResponseDto? Customer { get; set; }
    }
}
