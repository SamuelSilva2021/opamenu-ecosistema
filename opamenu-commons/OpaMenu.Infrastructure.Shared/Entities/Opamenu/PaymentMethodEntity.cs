using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    [Table("payment_methods")]
    public class PaymentMethodEntity
    {
        /// <summary>
        /// Primary key da entidade.
        /// </summary>
        [Column(name: "id")]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Nome do método de pagamento.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// slus do método de pagamento.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("slug")]
        public string Slug { get; set; } = string.Empty;
        /// <summary>
        /// Descrição do método de pagamento.
        /// </summary>
        [MaxLength(500)]
        [Column("description")]
        public string? Description { get; set; }
        /// <summary>
        /// Icone do método de pagamento.
        /// </summary>
        [MaxLength(500)]
        [Column("icon_url")]
        public string? IconUrl { get; set; }
        /// <summary>
        /// Situação do método de pagamento.
        /// </summary>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Método de pagamento disponível para uso online.
        /// </summary>
        [Column("is_online")]
        public bool IsOnline { get; set; }
        /// <summary>
        /// Ordem de exibição do método de pagamento.
        /// </summary>
        [Column("display_order")]
        public int DisplayOrder { get; set; }
        /// <summary>
        /// Data de criação da entidade.
        /// </summary>
        [Column(name: "created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Data da ultima atualização da entidade.
        /// </summary>
        [Column(name: "updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<TenantPaymentMethodEntity> TenantConfiguredMethods { get; set; } = new List<TenantPaymentMethodEntity>();
    }
}

