using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Grupo de acesso para usuários
    /// </summary>
    public class AccessGroupEntity
    {
        /// <summary>
        /// ID único do grupo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do grupo
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do grupo de acesso
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Código único do grupo
        /// </summary>
        [MaxLength(50)]
        public string? Code { get; set; }

        /// <summary>
        /// ID do tenant
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// ID do tipo de grupo
        /// </summary>
        public Guid? GroupTypeId { get; set; }

        /// <summary>
        /// Tipo do grupo
        /// </summary>
        public GroupTypeEntity GroupType { get; set; } = null!;

        /// <summary>
        /// Se o grupo está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<AccountAccessGroupEntity> AccountAccessGroups { get; set; } = new List<AccountAccessGroupEntity>();
        public virtual ICollection<RoleAccessGroupEntity> RoleAccessGroups { get; set; } = new List<RoleAccessGroupEntity>();

    }
}

