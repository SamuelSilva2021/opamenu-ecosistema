using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant
{
    /// <summary>
    /// Representa as informaÃ§Ãµes comerciais e de exibiÃ§Ã£o de um tenant (loja)
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
        /// DescriÃ§Ã£o "Sobre" da loja
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// URL do perfil do Instagram
        /// </summary>
        [MaxLength(255)]
        public string? InstagramUrl { get; set; }

        /// <summary>
        /// URL da pÃ¡gina do Facebook
        /// </summary>
        [MaxLength(255)]
        public string? FacebookUrl { get; set; }

        /// <summary>
        /// NÃºmero do WhatsApp para contato/pedidos (apenas nÃºmeros)
        /// </summary>
        [MaxLength(20)]
        public string? WhatsappNumber { get; set; }

        /// <summary>
        /// HorÃ¡rio de funcionamento em formato JSON
        /// Ex: [{"day": 1, "open": "18:00", "close": "23:00"}, ...]
        /// </summary>
        public string? OpeningHours { get; set; }

        /// <summary>
        /// MÃ©todos de pagamento aceitos para exibiÃ§Ã£o em formato JSON
        /// Ex: ["Dinheiro", "PIX", "CartÃ£o de CrÃ©dito"]
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

