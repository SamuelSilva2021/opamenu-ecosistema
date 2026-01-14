using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    public class TenantCustomerEntity
    {
        /// <summary>
        /// Identificador unico do cliente do tenant.
        /// </summary>
        [Column(name: "id")]
        public Guid Id { get; set; }
        /// <summary>
        /// Identificador do tenant ao qual o cliente pertence.
        /// </summary>
        [Column(name: "tenant_id")]
        public Guid TenantId { get; set; }
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        [Column(name: "customer_id")]
        public Guid CustomerId { get; set; }
        /// <summary>
        /// nome de exibição do cliente para o tenant.
        /// </summary>
        [Column(name: "display_name")]
        public string? DisplayName { get; set; }
        /// <summary>
        /// Gets or sets optional notes or comments associated with the entity.
        /// </summary>
        [Column(name: "notes")]
        public string? Notes { get; set; }
        [Column(name: "first_purchase_at")]
        public DateTime? FirstPurchaseAt { get; set; }
        [Column(name: "last_purchase_at")]
        public DateTime? LastPurchaseAt { get; set; }
        [Column(name: "total_orders")]
        public decimal TotalOrders { get; set; }
        [Column(name: "created_at")]
        public DateTime CreatedAt { get; set; }
        [Column(name: "updated_at")]
        public DateTime UpdatedAt { get; set; }
        public CustomerEntity Customer { get; set; } = null!;
    }
}

