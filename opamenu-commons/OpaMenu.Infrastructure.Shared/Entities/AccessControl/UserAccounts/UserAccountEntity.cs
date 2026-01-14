using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts
{
    /// <summary>
    /// Entidade que representa um usuário no sistema de controle de acesso
    /// </summary>
    public class UserAccountEntity
    {
        /// <summary>
        /// ID único do usuário
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID do tenant ao qual o usuário pertence
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Nome de usuário (username)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash da senha
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Primeiro nome do usuário
        /// </summary>
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Sobrenome do usuário
        /// </summary>
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Número de telefone
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Se o usuário está ativo
        /// </summary>
        public EUserAccountStatus Status { get; set; } = EUserAccountStatus.Ativo;

        /// <summary>
        /// Se o email foi verificado
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Data de exclusão (soft delete)
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Última data de login
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Token para reset de senha
        /// </summary>
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Data de expiração do token de reset de senha
        /// </summary>
        public DateTime? PasswordResetExpiresAt { get; set; }

        /// <summary>
        /// Tenant ao qual o usuário pertence - removido navigation property (tabela está em outro banco)
        /// </summary>
        // public TenantEntity? Tenant { get; set; }
        // Navigation properties
        public virtual ICollection<AccountAccessGroupEntity> AccountAccessGroups { get; set; } = new List<AccountAccessGroupEntity>();
    }
}

