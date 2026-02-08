using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs
{
    /// <summary>
    /// DTO para atualização de Role
    /// </summary>
    public class RoleUpdateDTO
    {
        /// <summary>
        /// Nome do role
        /// </summary>
        [MaxLength(255)]
        public string? Name { get; set; }

        /// <summary>
        /// Descrição do role
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Código único do role
        /// </summary>
        [MaxLength(50)]
        public string? Code { get; set; }

        /// <summary>
        /// ID do tenant (opcional)
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// ID da aplicação (opcional)
        /// </summary>
        public Guid? ApplicationId { get; set; }

        /// <summary>
        /// Permissões a serem sincronizadas com o role
        /// </summary>
        public List<SimplifiedPermissionDTO>? Permissions { get; set; } = [];

        /// <summary>
        /// IDs dos grupos de acesso a serem sincronizados com o role
        /// </summary>
        public List<Guid>? AccessGroupIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Se o role está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}