namespace Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs
{
    /// <summary>
    /// DTO para resposta de permissão
    /// </summary>
    public class PermissionDTO
    {
        /// <summary>
        /// ID único da permissão
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome da permissão
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID do tenant
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Descrição da permissão
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Código da permissão
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// ID do módulo ao qual a permissão se aplica
        /// </summary>
        public Guid? ModuleId { get; set; }

        /// <summary>
        /// Nome do módulo
        /// </summary>
        public string? ModuleName { get; set; }

        /// <summary>
        /// Se a permissão está ativa
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
        /// Operações associadas à permissão
        /// </summary>
        public List<PermissionOperationDTO>? Operations { get; set; } = new List<PermissionOperationDTO>();
    }

}
