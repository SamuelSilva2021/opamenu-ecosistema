using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Entidade que representa a relação entre papéis e permissões (Simplificada)
    /// </summary>
    public class RolePermissionEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }

        /// <summary>
        /// Chave do módulo (ex: DASHBOARD, PRODUCT, ORDER)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ModuleKey { get; set; } = string.Empty;

        /// <summary>
        /// Lista de ações permitidas (ex: ["READ", "CREATE", "UPDATE", "DELETE"])
        /// </summary>
        public List<string> Actions { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public RoleEntity Role { get; set; } = null!;
    }
}

