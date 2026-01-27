using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    [Table("tenant_payment_methods")]
    public class TenantPaymentMethodEntity : BaseEntity
    {
        /// <summary>
        /// Id do método de pagamento.
        /// </summary>
        [Required]
        [Column("payment_method_id")]
        public Guid PaymentMethodId { get; set; }
        /// <summary>
        /// Entidade do método de pagamento.
        /// </summary>
        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethodEntity PaymentMethod { get; set; } = null!;
        /// <summary>
        /// Alias do método de pagamento para o inquilino.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("alias")]
        public string? Alias { get; set; }
        /// <summary>
        /// Situação 
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Configuração do método de pagamento em formato JSON.
        /// </summary>
        [Column("configuration")]
        public string? Configuration { get; set; }
        /// <summary>
        /// Ordem de exibição do método de pagamento.
        /// </summary>
        [Column("display_order")]
        public int DisplayOrder { get; set; }
    }
}

