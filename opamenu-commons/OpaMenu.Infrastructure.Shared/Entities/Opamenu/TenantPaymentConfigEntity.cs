using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    /// <summary>
    /// Configuração de pagamento do tenant
    /// </summary>
    /// <value></value>
    [Table("tenant_payment_configs")]
    public class TenantPaymentConfigEntity : BaseEntity
    {
        /// <summary>
        /// Provedor de pagamento
        /// </summary>
        /// <value></value>
        [Column("provider")]
        public EPaymentProvider Provider { get; set; }

        /// <summary>
        /// Método de pagamento (PIX, Crédito, Débito)
        /// </summary>
        [Column("payment_method")]
        public EPaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Chave PIX do tenant
        /// </summary>
        /// <value></value>
        [Column("pix_key")]
        public string PixKey { get; set; } = null!;

        /// <summary>
        /// ClientId do provedor de pagamento
        /// </summary>
        /// <value></value>
        [Column("client_id")]
        public string ClientId { get; set; } = null!;

        [Column("is_sandbox")]
        public bool? IsSandbox { get; set; }

        /// <summary>
        /// ClientSecret do provedor de pagamento
        /// </summary>
        /// <value></value>
        [Column("client_secret")]
        public string ClientSecret { get; set; } = null!; // criptografar

        /// <summary>
        /// Chave Pública do provedor de pagamento (Ex: Mercado Pago Public Key)
        /// </summary>
        [Column("public_key")]
        public string? PublicKey { get; set; }

        /// <summary>
        /// Access Token do provedor de pagamento (Ex: Mercado Pago Access Token)
        /// </summary>
        [Column("access_token")]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Indica se a configuração de pagamento está ativa
        /// </summary>
        /// <value></value>
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
