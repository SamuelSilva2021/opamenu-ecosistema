using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities
{
    [Table("tenant_payment_methods")]
    public class TenantPaymentMethodEntity : BaseEntity
    {
        [Required]
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethodEntity PaymentMethod { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [Column("alias")]
        public string? Alias { get; set; } // Nome personalizado (ex: "PIX da Loja")

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("configuration")]
        [MaxLength(500)]
        public string? Configuration { get; set; } // JSON para credenciais (Chave Pix, Tokens, etc)

        // Propriedade para definir ordem de exibiÃ§Ã£o especÃ­fica da loja
        [Column("display_order")]
        public int DisplayOrder { get; set; }
    }
}

