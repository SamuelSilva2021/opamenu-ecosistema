using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Domain.DTOs.Opamenu.Providers
{
    /// <summary>
    /// Represena um provedor de processamento de pix
    /// </summary>
    public sealed class PixProviderResultDto
    {
        /// <summary>
        /// Provedor
        /// </summary>
        public string Provider { get; init; } = default!; // GERENCIANET, MERCADOPAGO
        /// <summary>
        /// Id
        /// </summary>
        public string ProviderPaymentId { get; init; } = default!; // txid, id, charge_id
        /// <summary>
        /// Qr code copia e cola
        /// </summary>
        public string QrCode { get; init; } = default!; // copia e cola
        /// <summary>
        /// Qr code base 64
        /// </summary>
        public string? QrCodeBase64 { get; init; } // opcional
        /// <summary>
        /// Data de expiração do pix gerado
        /// </summary>
        public DateTime ExpiresAt { get; init; }
        /// <summary>
        /// Valor do pix
        /// </summary>
        public decimal Amount { get; init; }
        /// <summary>
        /// Moeda usada
        /// </summary>
        public string Currency { get; init; } = "BRL";
    }

}
