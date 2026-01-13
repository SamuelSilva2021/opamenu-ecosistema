namespace Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs
{
    /// <summary>
    /// DTO para resposta de relacionamento entre permissão e operação
    /// </summary>
    public class PermissionOperationDTO
    {
        /// <summary>
        /// ID do relacionamento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID da permissão
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// ID da operação
        /// </summary>
        public Guid OperationId { get; set; }

        /// <summary>
        /// Nome da permissão
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>
        /// Nome da operação
        /// </summary>
        public string OperationName { get; set; } = string.Empty;

        /// <summary>
        /// Código da operação
        /// </summary>
        public string OperationCode { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da operação
        /// </summary>
        public string OperationDescription { get; set; } = string.Empty;

        /// <summary>
        /// Se a operação está ativa na permissão
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Data de criação do relacionamento
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
