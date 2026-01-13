using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs
{
    /// <summary>
    /// DTO para criação de Role
    /// </summary>
    public class RoleCreateDTO
    {
        /// <summary>
        /// Nome do role
        /// </summary>
        [Required(ErrorMessage = "O nome do role é obrigatório")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

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
        /// IDs das permissões a serem atribuídas ao role
        /// </summary>
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();

        /// <summary>
        /// IDs dos grupos de acesso a serem atribuídos ao role
        /// </summary>
        public List<Guid> AccessGroupIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Se o role está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}