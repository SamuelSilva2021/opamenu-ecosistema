using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant
{
    /// <summary>
    /// Representa as informações comerciais de um tenant
    /// </summary>
    public class TenantBusinessEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [ForeignKey("TenantId")]
        [JsonIgnore]
        public virtual TenantEntity Tenant { get; set; } = null!;

        /// <summary>
        /// URL da logomarca da loja
        /// </summary>
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        /// <summary>
        /// URL do banner da loja
        /// </summary>
        [MaxLength(500)]
        public string? BannerUrl { get; set; }

        /// <summary>
        /// Descrição "Sobre" da loja
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL do perfil do Instagram
        /// </summary>
        [MaxLength(255)]
        public string? InstagramUrl { get; set; }

        /// <summary>
        /// URL da página do Facebook
        /// </summary>
        [MaxLength(255)]
        public string? FacebookUrl { get; set; }

        /// <summary>
        /// Número do WhatsApp para contato/pedidos
        /// </summary>
        [MaxLength(20)]
        public string? WhatsappNumber { get; set; }

        /// <summary>
        /// Horário de funcionamento em formato JSON
        /// Ex: [{"day": 1, "open": "18:00", "close": "23:00"}, ...]
        /// </summary>
        public string? OpeningHours { get; set; }

        /// <summary>
        /// Métodos de pagamento
        /// Ex: ["Dinheiro", "PIX", "Cartão de crédito"]
        /// </summary>
        public string? PaymentMethods { get; set; }

        /// <summary>
        /// Latitude para mapas
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude para mapas
        /// </summary>
        public double? Longitude { get; set; }
    }
}

