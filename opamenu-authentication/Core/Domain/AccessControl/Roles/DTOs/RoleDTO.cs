using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
namespace Authenticator.API.Core.Domain.AccessControl.Roles.DTOs
{
    /// <summary>
    /// DTO de resposta para Role
    /// </summary>
    public class RoleDTO
    {
        /// <summary>
        /// ID único do role
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do role
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do role
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Código único do role
        /// </summary>
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
        public bool IsActive { get; set; }

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Permissões associadas ao role
        /// </summary>
        public List<SimplifiedPermissionDTO> Permissions { get; set; } = [];

        /// <summary>
        /// Grupos de acesso associados ao role
        /// </summary>
        public IEnumerable<AccessGroupDTO> AccessGroups { get; set; } = new List<AccessGroupDTO>();
    }
}