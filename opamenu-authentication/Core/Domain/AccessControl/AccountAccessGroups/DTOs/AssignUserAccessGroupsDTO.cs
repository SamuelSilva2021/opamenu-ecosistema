using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.DTOs
{
    /// <summary>
    /// DTO para atribuição de grupos de acesso a um usuário
    /// </summary>
    public class AssignUserAccessGroupsDTO
    {
        /// <summary>
        /// Lista de IDs de grupos de acesso a serem atribuídos
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Informe pelo menos um grupo")]
        public List<Guid> AccessGroupIds { get; set; } = new();

        /// <summary>
        /// Data de expiração padrão para todos os vínculos (opcional)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }
}