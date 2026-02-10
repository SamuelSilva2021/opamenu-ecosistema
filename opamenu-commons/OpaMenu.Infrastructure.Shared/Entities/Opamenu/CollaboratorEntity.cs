using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Entities.Opamenu
{
    [Table("collaborators")]
    public class CollaboratorEntity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Column("type")]
        public ECollaboratorType Type { get; set; }

        [MaxLength(50)]
        [Column("role")]    
        public string? Role { get; set; }

        [Column("active")]
        public bool Active { get; set; } = true;

        // Vínculo opcional com usuário do sistema
        [Column("user_account_id")]
        public Guid? UserAccountId { get; set; }

        // Vínculo obrigatório com o Tenant (Loja)
        [Column("tenant_id")]
        public Guid TenantId { get; set; }
    }
}
