using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{

    /// <summary>
    /// Permissão no sistema
    /// </summary>
    public class PermissionEntity
    {
        /// <summary>
        /// ID único da permissão
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome da permissão (derivado do módulo)
        /// </summary>
        public string Name => Module?.Name ?? $"Permission_{Id}";

        /// <summary>
        /// ID do tenant
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Descrição da permissão (derivada do módulo)
        /// </summary>
        public string Description => Module?.Description ?? string.Empty;

        /// <summary>
        /// Código da permissão (derivado do módulo)
        /// </summary>
        public string? Code => Module?.Key;

        /// <summary>
        /// ID do módulo ao qual a permissão se aplica
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// Se a permissão está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        // Removido navigation property para Tenant - tabela está em outro banco (multi_tenant_db)
        // public TenantEntity? Tenant { get; set; }
        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
        public virtual ModuleEntity? Module { get; set; }
        public virtual ICollection<PermissionOperationEntity> PermissionOperations { get; set; } = new List<PermissionOperationEntity>();
    }
}

