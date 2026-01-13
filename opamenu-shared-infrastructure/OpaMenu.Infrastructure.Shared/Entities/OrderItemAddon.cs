using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    /// <summary>
    /// Representa um item adicional de um pedido.
    /// </summary>
    [Table("order_item_addons")]
    public class OrderItemAddon : BaseEntity
    {
        /// <summary>
        /// Identificador do item do pedido ao qual este item adicional estÃ¡ associado.
        /// </summary>
        [Column("order_item_id")]
        public int OrderItemId { get; set; }
        /// <summary>
        /// Identificador do addon associado a este item adicional do pedido.
        /// </summary>
        [Column("addon_id")]
        public int AddonId { get; set; }
        /// <summary>
        /// Nome do adcional associado a este item adicional do pedido.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("addon_name")]
        public string AddonName { get; set; } = string.Empty; // Snapshot do nome
        /// <summary>
        /// PreÃ§o unitÃ¡rio do addon no momento da adiÃ§Ã£o ao pedido.
        /// </summary>
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; } // Snapshot do preÃ§o
        /// <summary>
        /// Quantidade do addon adicionada ao item do pedido.
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; } = 1;
        /// <summary>
        /// Subtotal calculado para este item adicional do pedido (UnitPrice * Quantity).
        /// </summary>
        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
        public virtual OrderItemEntity OrderItem { get; set; } = null!;
        public virtual AddonEntity Addon { get; set; } = null!;
    }
}

