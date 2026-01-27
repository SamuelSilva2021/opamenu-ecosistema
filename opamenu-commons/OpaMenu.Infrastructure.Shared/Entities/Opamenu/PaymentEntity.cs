using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu;
/// <summary>
/// Tabela de pagamentos
/// </summary>
[Table("payments")]
public class PaymentEntity : BaseEntity
{
    /// <summary>
    /// Id do pedido
    /// </summary>
    [Required]
    [Column("order_id")]
    public Guid OrderId { get; set; }

    /// <summary>
    /// Id do método de pagamento
    /// </summary>
    [Column("payment_method_id")]
    public Guid? PaymentMethodId { get; set; }

    /// <summary>
    /// Método de pagamento
    /// </summary>
    /// <value></value>
    public virtual PaymentMethodEntity? PaymentMethod { get; set; }
    
    [Column("method")]
    public EPaymentMethod Method { get; set; }

    /// <summary>
    /// Provedor de pagamento
    /// </summary>
    /// <value></value>
    [Column("provider")]
    public EPaymentProvider? Provider { get; set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    /// <value></value>
    [Required]
    [Column("status")]
    public EPaymentStatus Status { get; set; }

    /// <summary>
    /// Valor do pagamento
    /// </summary>
    /// <value></value>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que 0")]
    [Column("amount", TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Id do pagamento no provedor
    /// </summary>
    /// <value></value>
    [Column("provider_payment_id")]
    public string? ProviderPaymentId { get; set; }

    /// <summary>
    /// Moeda do pagamento
    /// </summary>
    /// <value></value>
    [Required]
    [MaxLength(3)]
    [Column("currency")]
    public string Currency { get; set; } = "BRL";

    /// <summary>
    /// Código QR do pagamento
    /// </summary>
    /// <value></value>
    [Column("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>
    /// Data de expiração do código QR
    /// </summary>
    /// <value></value>
    [Column("qr_code_expiration_at")]
    public DateTime? QrCodeExpirationAt { get; set; }

    /// <summary>
    /// Id da transação no gateway de pagamento
    /// </summary>
    /// <value></value>
    [MaxLength(200)]
    [Column("gateway_transaction_id")]
    public string? GatewayTransactionId { get; set; }
    
    /// <summary>
    /// Resposta do gateway de pagamento
    /// </summary>
    /// <value></value>
    [MaxLength(1000)]
    [Column("gateway_response")]
    public string? GatewayResponse { get; set; }

    /// <summary>
    /// Data de pagamento
    /// </summary>
    /// <value></value>
    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Notas adicionais
    /// </summary>
    /// <value></value>
    [MaxLength(500)]
    [Column("notes")]
    public string? Notes { get; set; }
    
    // Navigation properties
    public virtual OrderEntity Order { get; set; } = null!;
    public virtual ICollection<PaymentRefundEntity> Refunds { get; set; } = new List<PaymentRefundEntity>();

    public static PaymentEntity CreatePix(Guid orderId, decimal amount, Guid tenantId)
    {
        return new PaymentEntity
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = orderId,
            Amount = amount,
            Method = EPaymentMethod.Pix,
            Status = EPaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AttachPixData(string provider, string providerPaymentId, string qrCode, string? qrCodeBase64, DateTime expiresAt)
    {
        // Try parse provider string to enum
        if (Enum.TryParse<EPaymentProvider>(provider, true, out var providerEnum))
        {
            Provider = providerEnum;
        }
        
        ProviderPaymentId = providerPaymentId;
        GatewayTransactionId = providerPaymentId; // Mapping redundant field
        QrCode = qrCode;
        QrCodeExpirationAt = expiresAt;
        Status = EPaymentStatus.Pending; // Re-affirm pending
    }

    public void ProcessWebhookResult(EPaymentStatus newStatus, DateTime? paidAt, string rawResponse)
    {
        // Só permite transição se não estiver finalizado (Paid, Failed, Cancelled)
        if (Status == EPaymentStatus.Paid || Status == EPaymentStatus.Failed || Status == EPaymentStatus.Cancelled)
            return;

        Status = newStatus;
        if (paidAt.HasValue)
        {
            PaidAt = paidAt.Value;
        }
        GatewayResponse = rawResponse;
        
        // Se falhou, poderia adicionar lógica extra
    }
}
