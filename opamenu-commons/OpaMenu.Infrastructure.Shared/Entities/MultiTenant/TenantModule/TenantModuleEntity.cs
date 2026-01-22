using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule
{
    /// <summary>
    /// Relacionamento entre Tenant e Módulo (Quais módulos o tenant ativou/comprou)
    /// </summary>
    public class TenantModuleEntity
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Indica se o módulo está habilitado para o tenant
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Configurações específicas do módulo para este tenant (JSON)
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string? Configuration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual TenantEntity Tenant { get; set; } = null!;
        public virtual ModuleEntity Module { get; set; } = null!;
    }
}
