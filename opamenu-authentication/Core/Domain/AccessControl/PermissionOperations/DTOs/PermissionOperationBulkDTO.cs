using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs
{
    /// <summary>
    /// DTO para operações em lote de permission operations
    /// </summary>
    public class PermissionOperationBulkDTO
    {
        /// <summary>
        /// ID da permissão
        /// </summary>
        [Required(ErrorMessage = "O ID da permissão é obrigatório")]
        public Guid PermissionId { get; set; }

        /// <summary>
        /// IDs das operações a serem associadas/desassociadas
        /// </summary>
        public List<Guid> OperationIds { get; set; } = new List<Guid>();
    }
}
