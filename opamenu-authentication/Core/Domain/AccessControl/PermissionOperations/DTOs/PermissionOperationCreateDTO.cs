using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs
{
    /// <summary>
    /// DTO para criação de relacionamento entre permissão e operação
    /// </summary>
    public class PermissionOperationCreateDTO
    {
        /// <summary>
        /// ID da permissão
        /// </summary>
        [Required(ErrorMessage = "O ID da permissão é obrigatório")]
        public Guid PermissionId { get; set; }

        /// <summary>
        /// ID da operação
        /// </summary>
        [Required(ErrorMessage = "O ID da operação é obrigatório")]
        public Guid OperationId { get; set; }

        /// <summary>
        /// Se a operação está ativa na permissão
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
