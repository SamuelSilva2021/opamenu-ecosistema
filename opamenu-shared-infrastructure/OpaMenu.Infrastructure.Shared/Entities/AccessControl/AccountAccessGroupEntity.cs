using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl
{
    /// <summary>
    /// Relacionamento entre conta de usuário e grupo de acesso
    /// </summary>
    public class AccountAccessGroupEntity
    {
        /// <summary>
        /// ID único do relacionamento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID da conta do usuário
        /// </summary>
        public Guid UserAccountId { get; set; }

        /// <summary>
        /// ID do grupo de acesso
        /// </summary>
        public Guid AccessGroupId { get; set; }

        /// <summary>
        /// Se o relacionamento está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Usuário que concedeu o acesso
        /// </summary>
        public Guid? GrantedBy { get; set; }

        /// <summary>
        /// Data em que o acesso foi concedido
        /// </summary>
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de expiração do relacionamento
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Data de criação do relacionamento
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual UserAccountEntity UserAccount { get; set; } = null!;
        public virtual AccessGroupEntity AccessGroup { get; set; } = null!;
    }
}

