using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Relacionamento entre role e grupo de acesso
    /// </summary>
    public class RoleAccessGroupEntity
    {
        /// <summary>
        /// ID do relacionamento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID do grupo de acesso
        /// </summary>
        public Guid AccessGroupId { get; set; }

        /// <summary>
        /// ID do role
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Se o relacionamento está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do relacionamento
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        /// <summary>
        /// Role relacionado
        /// </summary>
        public virtual RoleEntity Role { get; set; } = null!;

        /// <summary>
        /// Grupo de acesso relacionado
        /// </summary>
        public virtual AccessGroupEntity AccessGroup { get; set; } = null!;
    }
}

