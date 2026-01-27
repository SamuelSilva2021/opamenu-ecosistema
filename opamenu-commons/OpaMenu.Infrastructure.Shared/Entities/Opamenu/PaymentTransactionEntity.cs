using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu 
{
    /// <summary>
    /// Entidade de transação de pagamento
    /// </summary>
    [Table("payment_transaction")]
    public class PaymentTransactionEntity
    {
        /// <summary>
        /// Id da transação de pagamento
        /// </summary>
        /// <value></value>
        [Key]
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Id do pagamento
        /// </summary>
        /// <value></value>
        [Required]
        [Column("payment_id")]
        public Guid PaymentId { get; set; }

        /// <summary>
        /// Navegação para o pagamento
        /// </summary>
        /// <value></value>
        public PaymentEntity Payment { get; set; } = null!;

        /// <summary>
        /// Tipo de evento de pagamento
        /// </summary>
        /// <value></value>
        [Required]
        [Column("event_type")]
        public EPaymentEventType EventType { get; set; }

        /// <summary>
        /// Id do evento no provedor de pagamento
        /// </summary>
        /// <value></value>
        [Column("provider_event_id")]
        public string? ProviderEventId { get; set; }

        /// <summary>
        /// Payload bruto da transação de pagamento
        /// </summary>
        /// <value></value>
        [Column(TypeName = "jsonb")]
        public string RawPayload { get; set; } = "{}";

        /// <summary>
        /// Data de criação da transação de pagamento
        /// </summary>
        /// <value></value>
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}