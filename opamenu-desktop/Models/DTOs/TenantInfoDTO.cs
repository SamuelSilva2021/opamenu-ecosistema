using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Desktop.Models.DTOs
{
    /// <summary>
    /// Informações básicas do tenant
    /// </summary>
    public class TenantInfoDTO
    {
        /// <summary>
        /// ID do tenant
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do tenant
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Slug do tenant
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Domínio personalizado
        /// </summary>
        public string? CustomDomain { get; set; }
    }
}
