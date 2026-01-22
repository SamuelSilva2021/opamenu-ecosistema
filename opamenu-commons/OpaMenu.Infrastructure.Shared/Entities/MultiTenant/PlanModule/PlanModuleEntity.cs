using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule
{
    /// <summary>
    /// Relacionamento entre Plano e Módulo (Quais módulos estão disponíveis no plano)
    /// </summary>
    public class PlanModuleEntity
    {
        public Guid Id { get; set; }

        public Guid PlanId { get; set; }
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Indica se o módulo está incluso no plano (pode ser usado para soft delete ou toggle)
        /// </summary>
        public bool IsIncluded { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual PlanEntity Plan { get; set; } = null!;
        public virtual ModuleEntity Module { get; set; } = null!;
    }
}
