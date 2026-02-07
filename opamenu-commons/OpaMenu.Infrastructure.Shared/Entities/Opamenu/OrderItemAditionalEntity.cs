using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    /// <summary>
    /// Representa um item adicional de um pedido.
    /// </summary>
    [Table("order_item_aditionals")]
    public class OrderItemAditionalEntity : BaseEntity
    {
        /// <summary>
        /// Identificador do item do pedido ao qual este item adicional está associado.
        /// </summary>
        [Column("order_item_id")]
        public Guid OrderItemId { get; set; }
        /// <summary>
        /// Identificador do adicional associado a este item adicional do pedido.
        /// </summary>
        [Column("aditional_id")]
        public Guid AditionalId { get; set; }
        /// <summary>
        /// Nome do adicional associado a este item adicional do pedido.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("aditional_name")]
        public string AditionalName { get; set; } = string.Empty; // Snapshot do nome
        /// <summary>
        /// Preço unitário do adicional no momento da adição ao pedido.
        /// </summary>
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; } // Snapshot do preço
        /// <summary>
        /// Quantidade do adicional adicionada ao item do pedido.
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; } = 1;
        /// <summary>
        /// Subtotal calculado para este item adicional do pedido (UnitPrice * Quantity).
        /// </summary>
        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }
        public virtual OrderItemEntity OrderItem { get; set; } = null!;
        public virtual AditionalEntity Aditional { get; set; } = null!;
    }
}
