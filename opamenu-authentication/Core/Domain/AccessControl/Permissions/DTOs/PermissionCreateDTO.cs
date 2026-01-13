using System.ComponentModel.DataAnnotations;

namespace Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs
{
    /// <summary>
    /// DTO para criação de permissão
    /// </summary>
    public class PermissionCreateDTO
    {
        /// <summary>
        /// ID do tenant (opcional)
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// ID do módulo ao qual a permissão se aplica
        /// </summary>
        [Required(ErrorMessage = "O ID do módulo é obrigatório")]
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// IDs das operações associadas à permissão
        /// </summary>
        public List<Guid>? OperationIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Se a permissão está ativa
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
