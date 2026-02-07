using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Papel/função no sistema
    /// </summary>
    public class RoleEntity
    {
        /// <summary>
        /// ID único do papel
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do role ex: Administrador, Usuário
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do role ex: Papel com permissões administrativas, Papel com permissões de usuário comum
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Código único do role
        /// </summary>
        [MaxLength(50)]
        public string? Code { get; set; }

        /// <summary>
        /// ID do tenant
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// ID da aplicação
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Se o role está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Se é um role do sistema (padrão) que não pode ser excluído pelo tenant
        /// </summary>
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
        public ICollection<RoleAccessGroupEntity> RoleAccessGroups { get; set; } = new List<RoleAccessGroupEntity>();
    }
}

